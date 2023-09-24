[CmdletBinding()]
param (
    [switch]$DeleteShared = $false
)

# $mainRg = $env:PERFORMANCETEST_RG_NAME 
# $sharedRg = $env:PERFORMANCETEST_SHARED_RG_NAME 
$mainRg = $deploymentResults.performanceTestRgName
$sharedRg = $deploymentResults.sharedRgName

if (-not $mainRg) { 
    Write-Host("`$env:PERFORMANCETEST_RG_NAME is not set")
    exit
}

if ($DeleteShared) {
    if (-not $sharedRg) { 
        Write-Host("`$env:PERFORMANCETEST_SHARED_RG_NAME is not set. Set environment variable or don't use -DeleteShared flag")
        exit
    }
}

Remove-AzResourceGroup -Name $mainRg

if ($DeleteShared) {
    Remove-AzResourceGroup -Name $sharedRg 
}