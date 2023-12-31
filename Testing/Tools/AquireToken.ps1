#region Authentication
#We use the client credentials flow as an example. For production use, REPLACE the code below with your preferred auth method. NEVER STORE CREDENTIALS IN PLAIN TEXT!!!

#Variables to configure
$tenantID = "a9e2b040-93ef-4252-992e-0d9830029ae8" #your tenantID or tenant root domain
$appID = "0855a530-9f1d-499c-9bb0-dec3c9f5969e" #the GUID of your app.
$client_secret = "" #client secret for the app
$scope = "api://securedapi-gateway-ptst/.default"

#Prepare token request
$url = 'https://login.microsoftonline.com/' + $tenantId + '/oauth2/v2.0/token'

'https://login.microsoftonline.com/a9e2b040-93ef-4252-992e-0d9830029ae8/v2.0/.well-known/openid-configuration' 

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
    return 
}

$token = ($tokenRequest.Content | ConvertFrom-Json).access_token

$token

# $authHeader = @{
#    'Content-Type'='application\json'
#    'Authorization'="Bearer $token"
# }
#endregion Authentication