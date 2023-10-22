[CmdletBinding()]
param (
    [switch] $Force = $false
    , [switch] $DoNotDeployDocker= $false
    , $AppPlanSku="S1"
    , $GatewayInstanceNum=1
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

$result = New-AzSubscriptionDeployment `
    -Location westeurope `
    -TemplateFile "./../../Deployment/TestInfra/Templates/performance-test.bicep" `
    -TemplateParameterFile "./../../Deployment/TestInfra/Parameters/performance-test-westeurope.json" `
    -commonNameEnding $infraCommonNameEnding `
    -deployLatestFromDocker $deployFromDocker `
    -appPlanSku $AppPlanSku `
    -gatewayInstanceNum $GatewayInstanceNum
    #-Whatif

$global:debugDeploymentResult = $result

if ($result.Outputs -ne $null) {
    # convert ugly presentation of outputs to the object
    $output = @{}
    foreach ($h in $result.Outputs.GetEnumerator()) {
        $output.Add($h.Key, $h.Value.Value)
    }
    $output = $output | ConvertTo-Json | ConvertFrom-Json
    $global:deploymentResults = $output

    $global:deploymentResults | ConvertTo-Json -Depth 10
}

