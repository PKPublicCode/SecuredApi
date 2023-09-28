#$msg = Read-Host -Prompt "Enter message"
#$encmsg = [System.Web.HttpUtility]::UrlEncode($msg)
try {
    Invoke-WebRequest `
        -Uri "https://$($deploymentResults.gateway.hostEndpoint)/private/redirect/wildcard/warmup" `
        -Headers @{"X-SUBSCRIPTION-KEY"="5F39D492-A141-498A-AE04-76C6B77F246A"}
}
catch {
    $_.Exception.Response
}