#Quick temporary solution to run old working scripts

[CmdletBinding()]
param (
    [switch]$Prod = $false
)

$majorVer = "0.2"
$verFile = "$($PSScriptRoot)/ver.txt"
$strVer = Get-Content $verfile 
$build = [System.Decimal]::Parse($strVer)

if ($Prod) {
    #bump new ver
    $build++
    Set-Content $verFile $build

    $latestTag = "latest"
    $majorVerTag = "$($majorVer)"
    $minorVerTag = "$($majorVer).$($build)"

    Write-Host("Building Prod")
}
else {
    $build = $build + 1
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


# Generate ARM Template
Set-Location "$($PSScriptRoot)/../../Deployment/TestInfra/Templates"
$armOutput = "../GeneratedTemplates"
New-Item -ItemType Directory -Force -Path $armOutput
az bicep build --file isolated-env.bicep --outdir $armOutput

Pop-Location