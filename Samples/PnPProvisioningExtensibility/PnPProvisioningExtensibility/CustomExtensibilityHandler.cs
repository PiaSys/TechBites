using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Diagnostics;
using OfficeDevPnP.Core.Framework.Provisioning.Extensibility;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers.TokenDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PnPProvisioningExtensibility
{
    public class CustomExtensibilityHandler : IProvisioningExtensibilityHandler
    {
        public ProvisioningTemplate Extract(ClientContext ctx, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInformation, PnPMonitoredScope scope, string configurationData)
        {
            // No extraction for this custom provider
            return (template);
        }

        public IEnumerable<TokenDefinition> GetTokens(ClientContext ctx, ProvisioningTemplate template, string configurationData)
        {
            // No custom tokens for this custom provider
            return (null);
        }

        public void Provision(ClientContext ctx, ProvisioningTemplate template, ProvisioningTemplateApplyingInformation applyingInformation, TokenParser tokenParser, PnPMonitoredScope scope, string configurationData)
        {
            // Sample extensibility
            var web = ctx.Web;
            var list = web.Lists.GetByTitle("Contacts List");
            ctx.Load(list, l => l.EnableFolderCreation);
            ctx.ExecuteQueryRetry();

            // Show a message, if there is a message delegate configured
            if (applyingInformation.MessagesDelegate != null)
            {
                applyingInformation.MessagesDelegate("Updating target list", 
                    ProvisioningMessageType.Progress);
            }

            // Just update the EnableFolderCreation for the list
            list.EnableFolderCreation = true;
            list.Update();
            ctx.ExecuteQueryRetry();

            // Show a message, if there is a message delegate configured
            if (applyingInformation.MessagesDelegate != null)
            {
                applyingInformation.MessagesDelegate("Updated target list",
                    ProvisioningMessageType.Progress);
            }
        }
    }
}
