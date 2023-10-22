Write-Host "If script hands, then probably jmx file is incorrect. az doesn't handle errors"

$idEnding = Get-Date -Format "_ddMMyyyy_HHmmss"

Upload-Test "Echo" `
            $deploymentResults.echo.appServiceName `
            $deploymentResults.echo.hostEndpoint `
            "/echo/" `

Upload-Test "Gateway" `
    $deploymentResults.gateway.appServiceName `
    $deploymentResults.gateway.hostEndpoint `
    "/private/redirect/wildcard/" ` 

function Upload-Test ([string]$name, [string]$srvName, [string]$url, [string]$urlPath) {
    $testId = "$($name)$($idEnding)".ToLower()
    Write-Host("Deloying $($testId) ...")
        
        #No powershell wrapper at the moment of writing this script, so using native az
    az load test create `
        --test-id $testId `
        --load-test-resource $deploymentResults.loadTesting.Name `
        --resource-group $deploymentResults.performanceTestRgName `
        --display-name "$($name) - $srvName" `
        --description "$($name) $($testId)" `
        --test-plan "./Scenarios/Tests.jmx" `
        --engine-instances 1 `
        --env SRV_URL=$url `
            SRV_URL_PATH=$urlPath `
    #--no-wait
}
