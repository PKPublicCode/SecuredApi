[CmdletBinding()]
param (
    [switch] $Rebuild = $false
)

$gwApp = $deploymentResults.gateway.appServiceName
$echoApp = $deploymentResults.echo.appServiceName
$mainRg = $deploymentResults.performanceTestRgName

if (-not $gwApp) { 
    Write-Host("`deploymentResults is not set")
    exit
}

if (-not $echoApp) { 
    Write-Host("`deploymentResults is not set")
    exit
}

$zipPath = "./../../Build/Output/Gateway/build.zip"

$fileExists = Test-Path $zipPath
if ($Rebuild -or !$fileExists) {
    if ($fileExists) {
        Remove-Item $zipPath
    }
    # re-using (copy-paste of) old scrips...
    #build
    dotnet publish ./../../SecuredApi/WebApps/Gateway/WebApps.Gateway.csproj -c Release -o ./../../Build/Output/Gateway/bin

    #zip
    cd ./../../Build/Output/Gateway/bin
    zip -r ./../build.zip ./
    cd -
}
#deploy

Publish-AzWebApp -ResourceGroupName $mainRg -Name $gwApp -ArchivePath $zipPath -Force
Publish-AzWebApp -ResourceGroupName $mainRg -Name $echoApp -ArchivePath $zipPath -Force

# Alternative
# az webapp deploy --resource-group rg-secureapi-ptst-weeu --name app-apigateway-ptst-weeu --src-path ./Build/Output/Gateway/build.zip --type=zip
# az webapp deploy --resource-group rg-secureapi-ptst-weeu --name app-echosrv-ptst-weeu --src-path ./Build/Output/Gateway/build.zip --type=zip

