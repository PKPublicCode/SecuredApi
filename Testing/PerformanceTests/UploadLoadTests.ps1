Write-Host "If script hands, then probably jmx file is incorrect. az doesn't handle errors"
# !!!! Warning: passing secret to Load tests parameters !!! Need to fix

$idEnding = Get-Date -Format "_ddMMyyyy_HHmmss"

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
            SECAPI_IT_MAIN__SpiClient__ClientId=$env:SECAPI_IT_MAIN__SpiClient__ClientId `
            SECAPI_IT_MAIN__SpiClient__TenantId=$env:SECAPI_IT_MAIN__SpiClient__TenantId `
            SECAPI_IT_MAIN__SpiClient__ClientSecret=$env:SECAPI_IT_MAIN__SpiClient__ClientSecret `
            SECAPI_IT_MAIN__SpiClient__Scope=$env:SECAPI_IT_MAIN__SpiClient__Scope `
        #--no-wait
}

Upload-Test "Echo" `
            $deploymentResults.echo.appServiceName `
            $deploymentResults.echo.hostEndpoint `
            "/echo/delay/"

Upload-Test "Gateway_ApiKey" `
            $deploymentResults.gateway.appServiceName `
            $deploymentResults.gateway.hostEndpoint `
            "/private_api_key/redirect/wildcard/" 

Upload-Test "Gateway_OAuth" `
            $deploymentResults.gateway.appServiceName `
            $deploymentResults.gateway.hostEndpoint `
            "/private_oauth/redirect/wildcard/" 
