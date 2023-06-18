
$AccountName = $env:PERFORMANCETEST_CONFIGSTORAGE_NAME
$GatewayContainerName = $env:PERFORMANCETEST_GATEWAY_CONFIGBLOBCONTAINENR_NAME
$EchoContainerName = $env:PERFORMANCETEST_ECHOSRV_CONFIGBLOBCONTAINENR_NAME
$rgName = $env:PERFORMANCETEST_SHARED_RG_NAME
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

$Context = New-AzStorageContext -StorageAccountName $AccountName

# upload a file to the default account (inferred) access tier
$echoConfig = @{
    File             = './RoutingConfigs/EchoRouting/routing-config.json'
    Container        = $EchoContainerName
    Blob             = "routing-config.json"
    Context          = $Context
    StandardBlobTier = 'Hot'
  }

Set-AzStorageBlobContent @echoConfig

  # upload a file to the default account (inferred) access tier
$gatewayConfig = @{
  File             = './RoutingConfigs/GatewayRouting/routing-config.json'
  Container        = $GatewayContainerName
  Blob             = "routing-config.json"
  Context          = $Context
  StandardBlobTier = 'Hot'
}

Set-AzStorageBlobContent @gatewayConfig