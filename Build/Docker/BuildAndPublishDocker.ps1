#Quick temporary solution to run old working scripts

[CmdletBinding()]
param (
    [switch]$Prod = $false,
)

$verFile = "$($PSScriptRoot)/ver.txt"
$strVer = Get-Content $verfile 
$ver = [System.Decimal]::Parse($strVer)

$latest = "candidate"
if ($Prod) {
    #bump new ver
    $ver++
    Set-Content $verFile $ver
    
    #set latest tag
    $latest = "latest"

    Write-Host("Building Prod")
}
else {
    Write-Host("Building Candidate")
}

Push-Location
Set-Location "$($PSScriptRoot)/../../SecuredApi"

docker build `
    -t pkruglov/securedapi.gateway:0.1.$($ver) `
    -t pkruglov/securedapi.gateway:$($latest) `
    -f ../Build/Docker/dockerfile `
    .

docker push pkruglov/securedapi.gateway --all-tags 

Pop-Location