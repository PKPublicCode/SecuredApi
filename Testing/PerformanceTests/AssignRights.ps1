[CmdletBinding()]
param (
    $accountName,
    $rgName
)

if (($accountName -eq $null) && ($rgName -eq $null)) {
    $accountName = $deploymentResults.configStorageName
    $rgName = $deploymentResults.sharedRgName
}

$StorageScope = (Get-AzStorageAccount -ResourceGroupName $rgName -Name $AccountName).Id
$objId = (Get-AzADUser).Id 

#Assign rights to upload blobs
$existinngAssignement = Get-AzRoleAssignment `
   -ObjectId $objId `
   -RoleDefinitionName "Storage Blob Data Owner" `
   -Scope $StorageScope

if (-not $existinngAssignement) {
    New-AzRoleAssignment -ObjectId $objId `
      -RoleDefinitionName "Storage Blob Data Owner" `
      -Scope $StorageScope
}

"Warning!!! Applying assignements could take several minutes!!!!"
