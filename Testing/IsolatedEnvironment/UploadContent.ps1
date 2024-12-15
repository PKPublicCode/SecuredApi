[CmdletBinding()]
param (
  [string] $ConfigsPath,
  [string] $StaticContentPath
)

# Example to appload placeholder content:
# ./UploadContent.ps1 -ConfigsPath ./PlaceholderContent/Configuration -StaticContentPath ./PlaceholderContent/StaticContent

./../Tools/UploadFolder.ps1 -StorageAccount $deploymentResults.configStorageName `
                            -ContainerName $deploymentResults.gateway.blobs.configuration.name `
                            -SrcPath $ConfigsPath

./../Tools/UploadFolder.ps1 -StorageAccount $deploymentResults.configStorageName `
                            -ContainerName $deploymentResults.gateway.blobs.staticContent.name `
                            -SrcPath $StaticContentPath 