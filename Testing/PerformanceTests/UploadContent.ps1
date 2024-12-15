$AccountName = $deploymentResults.configStorageName

$contentBasePath = './../CommonContent'

./../Tools/UploadFolder.ps1 -StorageAccount $AccountName `
                            -ContainerName $deploymentResults.gateway.blobs.staticContent.name `
                            -SrcPath "$($contentBasePath)/StaticFiles" 

./../Tools/UploadFolder.ps1 -StorageAccount $AccountName `
                            -ContainerName $deploymentResults.gateway.blobs.subscriptionKeys.name `
                            -SrcPath "$($contentBasePath)/Subscriptions/Keys" 

./../Tools/UploadFolder.ps1 -StorageAccount $AccountName `
                            -ContainerName $deploymentResults.gateway.blobs.consumers.name `
                            -SrcPath "$($contentBasePath)/Subscriptions/Consumers" 

./../Tools/UploadFile.ps1 -StorageAccount $AccountName `
                            -ContainerName $deploymentResults.echo.blobs.configuration.name `
                            -SrcFile "$($contentBasePath)/Configuration/routing-config-echo.json" `
                            -BlobName "routing-config.json"


./../Tools/UploadFile.ps1 -StorageAccount $AccountName `
                            -ContainerName $deploymentResults.gateway.blobs.configuration.name `
                            -SrcFile "$($contentBasePath)/Configuration/routing-config-gateway.json" `
                            -BlobName "routing-config.json"

