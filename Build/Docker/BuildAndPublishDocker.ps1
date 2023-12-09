#Quick temporary solution to run old working scripts

[CmdletBinding()]
param (
    [switch]$Prod = $false
)

$majorVer = "0.1"
$verFile = "$($PSScriptRoot)/ver.txt"
$strVer = Get-Content $verfile 
$build = [System.Decimal]::Parse($strVer)

$latestTag = "rc"
$majorVerTag = "$($majorVer).rc"
$minorVerTag = "$($majorVer).$($build).rc"
if ($Prod) {
    $latestTag = "latest"
    $majorVerTag = "$($majorVer)"
    $minorVerTag = "$($majorVer).$($build)"

    #bump new ver
    # $build++
    # Set-Content $verFile $ver

    Write-Host("Building Prod")
}
else {
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

Pop-Location