# Source is here https://www.kallemarjokorpi.fi/blog/how-to-handle-interactive-login-in-powershell-and-query-api.html

$connectionDetails = @{
    'TenantId'    = 'a9e2b040-93ef-4252-992e-0d9830029ae8'
    'ClientId'    = '0855a530-9f1d-499c-9bb0-dec3c9f5969e'
    'Interactive' = $true
    #'Scopes' = 'api://securedapi-gateway-ptst/EchosSrv.Admin'
    'Scopes' = 'api://securedapi-gateway-ptst/EchoSrv.ReadWrite'
}

$token = Get-MsalToken @connectionDetails

$token.AccessToken