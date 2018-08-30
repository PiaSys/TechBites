$clearPassword = "***"
$password = $clearPassword | ConvertTo-SecureString -AsPlainText -Force
$cert = New-PnPAzureCertificate -CommonName "KeyVaultDemoConsumer" -ValidYears 5 -Out C:\temp\KeyVaultDemoConsumer.pfx -CertificatePassword $password
$cert.KeyCredentials | clip
$cert.Thumbprint | clip

$cert = Get-PnPAzureCertificate -CertificatePath C:\temp\KeyVaultDemoConsumer.pfx -CertificatePassword $password
$cert.KeyCredentials | clip
