[CmdletBinding()]
param (
    [string] $productName,
    [string] $commonNameEnding,
    [string] $rgName,
    [string] $location,
    [switch] $Force = $false
    , [ValidateSet("latest", "rc")]
    [string] $dockerTag = "latest"
    , [ValidateSet("S1", "P0V3", "P1V3")]
    [string] $AppPlanSku = "P0V3"
    , $GatewayInstanceNum = 1
)

$infraCommonNameEnding = $commonNameEnding
# if (-not $infraCommonNameEnding) { 
#     if (-not $Force) {
#         Write-Host("Environment variable SECURED_API_NAME_ENDING is not set. To set existing, execute: `$env:SECURED_API_NAME_ENDING=<your ending>")
#         Write-Host("Alternatively execute script with -force option, to generate new one: ./Deploy.ps1 -Force")
#         exit
#     }
#     $infraCommonNameEnding = $(-join (((48..57)+(65..90)+(97..122)) * 80 |Get-Random -Count 6 |%{[char]$_}))
#     Write-Host("New ending is generated")
#     $env:SECURED_API_NAME_ENDING = $infraCommonNameEnding
# }

if (!(Get-AzContext)) {
    Connect-AzAccount | Out-Null
    $context = Get-AzContext
    Write-Host("Connected to: $($context.Name)")
}

Write-Host("Creating environment with ending $infraCommonNameEnding")

$result = New-AzDeployment `
    -location $location `
    -TemplateFile "./../../Deployment/TestInfra/Templates/isolated-env.bicep" `
    -resourcesLocation $location `
    -productName $productName `
    -commonNameEnding $infraCommonNameEnding `
    -rgName $rgName `
    -dockerTag "$($dockerTag)" `
    -appPlanSku $AppPlanSku `
    -gatewayInstanceNum $GatewayInstanceNum `
    #-Whatif

$global:debugDeploymentResult = $result

if ($null -ne $result.Outputs) {
    # convert ugly presentation of outputs to the object
    $output = @{}
    foreach ($h in $result.Outputs.GetEnumerator()) {
        $output.Add($h.Key, $h.Value.Value)
    }
    $output = $output | ConvertTo-Json | ConvertFrom-Json
    $global:deploymentResults = $output

    $global:deploymentResults | ConvertTo-Json -Depth 10

#     "
# Copy-paste following to your .bash_profile:
# export SECAPI_IT_GW__Subscriptions__Keys__FileAccess__Rbac__Uri=`"$($output.gateway.blobs.subscriptionKeys.url)`"
# export SECAPI_IT_GW__Consumers__FileAccess__Rbac__Uri=`"$($output.gateway.blobs.consumers.url)`"
# export SECAPI_IT_GW__RoutingEngine__FileAccess__Rbac__Uri=`"$($output.gateway.blobs.configuration.url)`"
# export SECAPI_IT_GW__StaticContent__FileAccess__Rbac__Uri=`"$($output.gateway.blobs.staticContent.url)`"

# export SRV_URL_PATH=`"/api/jwt/basic_features/delay`"
# export SRV_URL=`"$($output.gateway.hostEndpoint)`"
#     "

}

