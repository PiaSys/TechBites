using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.WebKey;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace KeyVaultConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var clientId = ConfigurationManager.AppSettings["ClientId"];
            var certificateThumbprint = ConfigurationManager.AppSettings["CertificateThumbprint"];
            var certificateStoreName = ConfigurationManager.AppSettings["CertificateStoreName"];
            var certificateStoreLocation = ConfigurationManager.AppSettings["CertificateStoreLocation"];
            var vaultAddress = ConfigurationManager.AppSettings["VaultAddress"];

            var certificate = FindCertificateByThumbprint(certificateThumbprint, certificateStoreName, certificateStoreLocation);
            var assertionCert = new ClientAssertionCertificate(clientId, certificate);

            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(
                   (authority, resource, scope) => GetAccessTokenAsync(authority, resource, scope, assertionCert)));

            var key = Guid.NewGuid().ToString();

            AddItem(vaultAddress, keyVaultClient, key).GetAwaiter().GetResult();
            var tags = ReadItem(vaultAddress, keyVaultClient, key).GetAwaiter().GetResult();

            foreach (var t in tags)
            {
                Console.WriteLine($"{t.Key}: {t.Value}");
            }
        }

        private static async Task<IDictionary<String, String>> ReadItem(string vaultAddress, KeyVaultClient keyVaultClient, String key)
        {
            var retrievedKey = await keyVaultClient.GetKeyAsync(vaultAddress, key);
            return(retrievedKey.Tags);
        }

        private static async Task AddItem(string vaultAddress, KeyVaultClient keyVaultClient, String key)
        {
            var tags = new Dictionary<String, String>();
            tags.Add("Key1", "Value1");
            tags.Add("Key2", "Value2");

            var createdKey = await keyVaultClient.CreateKeyAsync(vaultAddress, key, JsonWebKeyType.Rsa, tags: tags);
        }

        /// <summary>
        /// Helper function to load an X509 certificate
        /// </summary>
        /// <param name="certificateThumbprint">Thumbprint of the certificate to be loaded</param>
        /// <param name="certificateStoreName">Store name of the certificate to be loaded</param>
        /// <param name="certificateStoreLocation">Store location of the certificate to be loaded</param>
        /// <returns>X509 Certificate</returns>
        /// <remarks>All input arguments are required, missing of an input argument result in an ArgumentNullException</remarks>
        public static X509Certificate2 FindCertificateByThumbprint(string certificateThumbprint, string certificateStoreName, string certificateStoreLocation)
        {
            if (String.IsNullOrEmpty(certificateThumbprint))
                throw new System.ArgumentNullException(nameof(certificateThumbprint));

            if (String.IsNullOrEmpty(certificateStoreName))
                throw new System.ArgumentNullException(nameof(certificateStoreName));

            if (String.IsNullOrEmpty(certificateStoreLocation))
                throw new System.ArgumentNullException(nameof(certificateStoreLocation));

            X509Certificate2 appOnlyCertificate = null;

            Enum.TryParse(certificateStoreName, out StoreName storeName);
            Enum.TryParse(certificateStoreLocation, out StoreLocation storeLocation);

            X509Store certStore = new X509Store(storeName, storeLocation);
            certStore.Open(OpenFlags.ReadOnly);

            X509Certificate2Collection certCollection = certStore.Certificates.Find(
                    X509FindType.FindByThumbprint,
                    certificateThumbprint,
                    false);

            // Get the first cert with the thumbprint
            if (certCollection.Count > 0)
            {
                appOnlyCertificate = certCollection[0];
            }
            certStore.Close();

            if (appOnlyCertificate == null)
            {
                throw new System.Exception(
                        string.Format("Could not find the certificate with thumbprint {0} in certificate store {1} in location {2}.", certificateThumbprint, certificateStoreName, certificateStoreLocation));
            }

            return appOnlyCertificate;
        }

        private static async Task<string> GetAccessTokenAsync(string authority, string resource, string scope, ClientAssertionCertificate assertionCert)
        {
            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
            var result = await context.AcquireTokenAsync(resource, assertionCert).ConfigureAwait(false);

            return result.AccessToken;
        }
    }
}
