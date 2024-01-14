#$msg = Read-Host -Prompt "Enter message"
#$encmsg = [System.Web.HttpUtility]::UrlEncode($msg)
$result = $null
$duration = Measure-Command -Expression { 
        try {
            $result = Invoke-WebRequest `
            -Uri "https://$($deploymentResults.gateway.hostEndpoint)/private_api_key/redirect/wildcard/warmup" `
            -Headers @{"X-SUBSCRIPTION-KEY"="5F39D492-A141-498A-AE04-76C6B77F246A"}
        }
        catch {
            $result = $_.Exception.Response
            Write-Error -Message $_.ErrorDetails.Message
        }
    }
$result
"StatusCode: $([int]$result.StatusCode)"
"Duration: $($duration.Microseconds)" 