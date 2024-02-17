$AccountName = $deploymentResults.configStorageName
$rgName = $deploymentResults.sharedRgName

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

$componentTestFolder = './../../SecuredApi/Apps/Gateway.ComponentTests/TestEnvironment'
$integrationTestsFolder = './../../SecuredApi/WebApps/Gateway.IntegrationTests/TestEnvironment'

Upload-Folder "$($componentTestFolder)/Subscriptions/Keys" `
            $deploymentResults.gateway.blobs.subscriptionKeys.name

Upload-Folder "$($componentTestFolder)/Subscriptions/Consumers" `
            $deploymentResults.gateway.blobs.consumers.name

Upload-Blob "$($integrationTestsFolder)/Configuration/routing-config-echo.json" ` `
                $deploymentResults.echo.blobs.configuration.name `
                "routing-config.json"

Upload-Blob "$($integrationTestsFolder)/Configuration/routing-config-gateway.json" `
                $deploymentResults.gateway.blobs.configuration.name `
                "routing-config.json"

