$AccountName = $deploymentResults.configStorageName
$rgName = $deploymentResults.sharedRgName

#Assign rights to upload blobs
./AssignRights.ps1 $AccountName $rgName 

$context = New-AzStorageContext -StorageAccountName $AccountName

function Upload-Blob([string]$filePath, [string]$containerName) {
  $blobName = Split-Path $filePath -leaf

  Set-AzStorageBlobContent -File $filePath `
                        -Container $containerName `
                        -Blob $blobName `
                        -Context $context `
                        -Force
}

function Upload-Folder ([string]$folder, [string]$container) {
  $files = Get-ChildItem $folder -File 
  foreach($file in $files) {
    Upload-Blob $file.FullName $container
  }
}

$componentTestFolder = './../../SecuredApi/ComponentTests/ComponentTests.Gateway/TestEnvironment'

Upload-Folder "$($componentTestFolder)/Subscriptions/Keys" `
            $deploymentResults.gateway.blobs.subscriptionKeys.name

Upload-Folder "$($componentTestFolder)/Subscriptions/Consumers" `
            $deploymentResults.gateway.blobs.consumers.name

Upload-Blob './RoutingConfigs/EchoRouting/routing-config.json' `
                $deploymentResults.echo.blobs.configuration.name

Upload-Blob "$($componentTestFolder)/Configuration/routing-config.json" `
                $deploymentResults.gateway.blobs.configuration.name

