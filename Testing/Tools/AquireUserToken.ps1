# Source is here https://www.kallemarjokorpi.fi/blog/how-to-handle-interactive-login-in-powershell-and-query-api.html

$connectionDetails = @{
    'TenantId'    = $env:SECURED_API_INTEGRATION_TESTS__SpiClient__TenantId 
    'ClientId'    = $env:SECURED_API_INTEGRATION_TESTS__SpiClient__ClientId 
    'Interactive' = $true
    'Scopes' = $env:SECURED_API_INTEGRATION_TESTS__SpiClient__Scope
}

$token = Get-MsalToken @connectionDetails

$token.AccessToken