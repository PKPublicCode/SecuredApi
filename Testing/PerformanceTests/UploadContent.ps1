$AccountName = $deploymentResults.configStorageName

$context = New-AzStorageContext -StorageAccountName $AccountName

function Upload-Blob([string]$filePath, [string]$containerName, [string]$blobName = $null) {
  if ($blobName -eq $null) {
    $blobName = Split-Path $filePath -leaf
  }

  Set-AzStorageBlobContent -File $filePath `
                        -Container $containerName `
                        -Blob $blobName `
                        -Context $context `
                        -Properties @{"CacheControl" = "no-store"}  `
                        -Force                     
}

function Upload-Folder ([string]$folder, [string]$container) {
  $files = Get-ChildItem $folder -File 
  foreach($file in $files) {
    Upload-Blob $file.FullName $container
  }
}

$contentBasePath = './../CommonContent'

Upload-Folder "$($contentBasePath)/Subscriptions/Keys" `
            $deploymentResults.gateway.blobs.subscriptionKeys.name

Upload-Folder "$($contentBasePath)/Subscriptions/Consumers" `
            $deploymentResults.gateway.blobs.consumers.name

Upload-Blob "$($contentBasePath)/Configuration/routing-config-echo.json" ` `
                $deploymentResults.echo.blobs.configuration.name `
                "routing-config.json"

Upload-Blob "$($contentBasePath)/Configuration/routing-config-gateway.json" `
                $deploymentResults.gateway.blobs.configuration.name `
                "routing-config.json"

