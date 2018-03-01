Connect-PnPOnline "https://piasysdev.sharepoint.com/sites/PnPProvisioningExtensibility" -Credentials PiaSysDev-Paolo

$handler = New-PnPExtensibilityHandlerObject `
    -Assembly "PnPProvisioningExtensibility, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" `
    -Type PnPProvisioningExtensibility.CustomExtensibilityHandler

Apply-PnPProvisioningTemplate -Path C:\github\TechBites\Samples\PnPProvisioningExtensibility\PnPProvisioningExtensibility\SampleTemplate.xml `
    -ExtensibilityHandlers @($handler)

