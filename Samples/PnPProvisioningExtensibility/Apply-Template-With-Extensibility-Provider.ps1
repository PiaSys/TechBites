Connect-PnPOnline "https://piasysdev.sharepoint.com/sites/PnPProvisioningExtensibility" -Credentials PiaSysDev-Paolo

Apply-PnPProvisioningTemplate -Path C:\github\TechBites\Samples\PnPProvisioningExtensibility\PnPProvisioningExtensibility\SampleTemplate.xml
