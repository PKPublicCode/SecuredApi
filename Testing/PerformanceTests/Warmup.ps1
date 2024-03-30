[CmdletBinding()]
param (
    [switch] $ApiKey = $false
)

if ($ApiKey) {
    $path = 'api/api_key'
    $authHeader = @{"X-SUBSCRIPTION-KEY"="5F39D492-A141-498A-AE04-76C6B77F246A"}
}
else {
    $token = ../Tools/AquireSpiToken.ps1
    $authHeader = @{"Authorization"="Bearer $token"}
    $path = 'api/jwt'
}

$result = $null
$duration = Measure-Command -Expression { 
        try {
            $result = Invoke-WebRequest `
            -Method post `
            -Uri "https://$($deploymentResults.gateway.hostEndpoint)/$path/basic_features/delay" `
            -Headers $authHeader
        }
        catch {
            $result = $_.Exception.Response
            Write-Error -Message $_.ErrorDetails.Message
        }
    }
$result
$result.Headers

"StatusCode: $([int]$result.StatusCode)"
"Duration: $($duration.Microseconds)" 