[CmdletBinding()]
param (
  [string] $StorageAccount,
  [string] $ContainerName,
  [string] $SrcPath
)

$basePathLen = "$(Resolve-Path $srcPath)".Length + 1
$files = Get-ChildItem $srcPath -File -Recurse
foreach($file in $files) {
  $blobName = $file.FullName.Substring($basePathLen)
  & "$PSScriptRoot/UploadFile.ps1" -storageAccount $storageAccount `
                    -containerName $containerName `
                    -srcFile $file.FullName `
                    -blobName $blobName
}

