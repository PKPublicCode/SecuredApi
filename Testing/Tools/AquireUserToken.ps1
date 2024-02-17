# Source is here https://www.kallemarjokorpi.fi/blog/how-to-handle-interactive-login-in-powershell-and-query-api.html

$connectionDetails = @{
    'TenantId'    = $env:SECAPI_IT_MAIN__SpiClient__TenantId 
    'ClientId'    = $env:SECAPI_IT_MAIN__SpiClient__ClientId 
    'Interactive' = $true
    'Scopes' = $env:SECAPI_IT_MAIN__SpiClient__Scope
}

$token = Get-MsalToken @connectionDetails

$token.AccessToken