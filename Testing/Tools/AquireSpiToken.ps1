#region Authentication
#We use the client credentials flow as an example. For production use, REPLACE the code below with your preferred auth method. NEVER STORE CREDENTIALS IN PLAIN TEXT!!!

#Variables to configure
$tenantID = $env:SECAPI_IT_MAIN__SpiClient__TenantId 
$appID = $env:SECAPI_IT_MAIN__SpiClient__ClientId 
$client_secret = $env:SECAPI_IT_MAIN__SpiClient__ClientSecret
$scope = $env:SECAPI_IT_MAIN__SpiClient__Scope

#Prepare token request
$url = 'https://login.microsoftonline.com/' + $tenantId + '/oauth2/v2.0/token'

#$wellKnown = 'https://login.microsoftonline.com/{tenantId}/v2.0/.well-known/openid-configuration' 
#How to load from KV: Get-AzKeyVaultSecret -VaultName ([System.Uri]$env:SecuredApi__Global__Keyvault).Host.Split('.')[0]  -Name 'name'  -AsPlainText

$body = @{
    grant_type = "client_credentials"
    client_id = $appID
    client_secret = $client_secret
    scope = $scope
}

#Obtain the token
Write-Verbose "Authenticating..."
try { $tokenRequest = Invoke-WebRequest -Method Post -Uri $url -ContentType "application/x-www-form-urlencoded" -Body $body -UseBasicParsing -ErrorAction Stop }
catch { 
    Write-Host "Unable to obtain access token, aborting..."; 
    $_.Exception.Response.Content 
    throw 
}

$token = ($tokenRequest.Content | ConvertFrom-Json).access_token

$token