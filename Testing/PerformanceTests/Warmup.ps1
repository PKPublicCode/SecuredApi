#$msg = Read-Host -Prompt "Enter message"
#$encmsg = [System.Web.HttpUtility]::UrlEncode($msg)
try {
    $duration = (Measure-Command -Expression { 
        $result = Invoke-WebRequest `
        -Uri "https://$($deploymentResults.gateway.hostEndpoint)/private/redirect/wildcard/warmup" `
        -Headers @{"X-SUBSCRIPTION-KEY"="5F39D492-A141-498A-AE04-76C6B77F246A"}
    }).Milliseconds
}
catch {
    $_.Exception.Response
}
$result
"Duration: $($duration)" 