[CmdletBinding()]
param (
  [string] $StorageAccount,
  [string] $ContainerName,
  [string] $SrcFile,
  [string] $BlobName
)

$AccountName = $StorageAccount

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

Upload-Blob $srcFile `
            $containerName `
            $blobName


