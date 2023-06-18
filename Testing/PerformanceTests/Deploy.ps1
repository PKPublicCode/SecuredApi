[CmdletBinding()]
param (
    [switch] $Force = $false
    , [switch] $DoNotDeployDocker= $false
    , $AppPlanSku="S1"
)

$infraCommonNameEnding = $env:SECURED_API_NAME_ENDING
if (-not $infraCommonNameEnding) { 
    if (-not $Force) {
        Write-Host("Environment variable SECURED_API_NAME_ENDING is not set. To set existing, execute: `$env:SECURED_API_NAME_ENDING=<your ending>")
        Write-Host("Alternatively execute script with -force option, to generate new one: ./Deploy.ps1 -Force")
        exit
    }
    $infraCommonNameEnding = $(-join (((48..57)+(65..90)+(97..122)) * 80 |Get-Random -Count 6 |%{[char]$_}))
    Write-Host("New ending is generated")
    $env:SECURED_API_NAME_ENDING = $infraCommonNameEnding
}

if (!(Get-AzContext)) {
    Connect-AzAccount | Out-Null
    $context = Get-AzContext
    Write-Host("Connected to: $($context.Name)")
}

Write-Host("Creating environment with ending $infraCommonNameEnding")

[bool] $deployFromDocker = !$doNotDeployDocker

$result = New-AzSubscriptionDeployment -Location westeurope `
    -TemplateFile "./../../Deployment/TestInfra/Templates/performance-test.bicep" `
    -TemplateParameterFile "./../../Deployment/TestInfra/Parameters/performance-test-westeurope.json" `
    -commonNameEnding $infraCommonNameEnding `
    -deployLatestFromDocker $deployFromDocker `
    -appPlanSku $AppPlanSku

$env:PERFORMANCETEST_RG_NAME = $result.Outputs.performanceTestRgName.Value
$env:PERFORMANCETEST_SHARED_RG_NAME = $result.Outputs.sharedRgName.Value
$env:PERFORMANCETEST_CONFIGSTORAGE_NAME = $result.Outputs.configStorageName.Value

$env:PERFORMANCETEST_GATEWAY_APPSERVICE_NAME = $result.Outputs.gateway.Value.appServiceName.Value
$env:PERFORMANCETEST_ECHOSRV_APPSERVICE_NAME = $result.Outputs.echo.Value.appServiceName.Value

$env:PERFORMANCETEST_GATEWAY_CONFIGBLOBCONTAINENR_NAME = $result.Outputs.gateway.Value.configBlobContainerName.Value
$env:PERFORMANCETEST_ECHOSRV_CONFIGBLOBCONTAINENR_NAME = $result.Outputs.echo.Value.configBlobContainerName.Value

Write-Host("Api Gateway:")
Write-Host("Host: $($result.Outputs.gateway.Value.hostEndpoint.Value)")
Write-Host("Configuration Contnainer Url: $($result.Outputs.gateway.Value.configBlobContainerUrl.Value)")
Write-Host("Configuration Blob Contnainer Name: $($result.Outputs.gateway.Value.configBlobContainerName.Value)")
Write-Host("Table Endpoint: $($result.Outputs.gateway.Value.tableEndpoint.Value)")
Write-Host("Tables: $($result.Outputs.gateway.Value.consumersTable.Value), `
            $($result.Outputs.gateway.Value.subscriptionsTable.Value), `
            $($result.Outputs.gateway.Value.subscriptionKeysTable.Value)")

Write-Host("Echo service:")
Write-Host("Host: $($result.Outputs.echo.Value.hostEndpoint.Value)")
Write-Host("Configuration Contnainer Url: $($result.Outputs.echo.Value.configBlobContainerUrl.Value)")
Write-Host("Configuration Blob Contnainer Name: $($result.Outputs.echo.Value.configBlobContainerName.Value)")
