[CmdletBinding()]
param (
    [string] $ProductName
    ,[string] $CommonNameEnding
    ,[string] $RgName
    ,[string] $Location
    ,[ValidateSet("latest", "rc")] [string] $dockerTag = "latest"
    ,[ValidateSet("S1", "P0V3", "P1V3")] [string] $AppPlanSku = "P0V3"
    ,$GatewayInstanceNum = 1
    ,[switch] $CreateResourceGroup = $false
)

$infraCommonNameEnding = $commonNameEnding

if (!(Get-AzContext)) {
    Connect-AzAccount | Out-Null
    $context = Get-AzContext
    Write-Host("Connected to: $($context.Name)")
}

Write-Host("Creating environment with ending $infraCommonNameEnding")

if ($createResourceGroup) {
    New-AzResourceGroup -Name $rgName -Location $location
}

$result = New-AzResourceGroupDeployment `
    -ResourceGroupName $rgName `
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
}

