#Quick temporary solution to run old working scripts

[CmdletBinding()]
param (
    [switch]$Prod = $false
)

$majorVer = "0.2"
$verFile = "$($PSScriptRoot)/ver.txt"
$strVer = Get-Content $verfile 
$build = [System.Decimal]::Parse($strVer)
#bump new ver
$build++

if ($Prod) {
    Set-Content $verFile $build

    $latestTag = "latest"
    $majorVerTag = "$($majorVer)"
    $minorVerTag = "$($majorVer).$($build)"

    Write-Host("Building Prod")
}
else {
    $latestTag = "rc"
    $majorVerTag = "$($majorVer).rc"
    $minorVerTag = "$($majorVer).$($build).rc"
    Write-Host("Building Candidate")
}

Push-Location
Set-Location "$($PSScriptRoot)/../../SecuredApi"

docker build `
    -t pkruglov/securedapi.gateway:$($majorVerTag) `
    -t pkruglov/securedapi.gateway:$($minorVerTag) `
    -t pkruglov/securedapi.gateway:$($latestTag) `
    -f ../Build/Docker/dockerfile `
    .

docker push pkruglov/securedapi.gateway --all-tags 

if ($Prod) {
    # Generate ARM Template
    Write-Host("Building ARM Templates")
    Set-Location "$($PSScriptRoot)/../../Deployment/TestInfra/Templates"
    $armOutput = "../GeneratedTemplates"
    New-Item -ItemType Directory -Force -Path $armOutput | Out-Null
    az bicep build --file isolated-env.bicep --outdir $armOutput
}

Pop-Location