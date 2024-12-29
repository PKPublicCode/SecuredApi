param resourcesLocation string
param productName string
param commonNameEnding string
param rgName string = toLower('rg-${productName}-${commonNameEnding}')
param appPlanSku string = 'S1'
param gatewayInstanceNum int = 1
param dockerTag string = 'latest'


module sharedResources './Modules/shared-resources.bicep' = {
  name: 'shared-resources'
  scope: resourceGroup(rgName)
  params: {
    baseName: productName
    nameEnding: commonNameEnding
    storageShortName: 'cfg${productName}'
  }
}

module gatewayService './Modules/gateway-service.bicep' = {
  name: 'main-service'
  scope: resourceGroup(rgName)
  params: {
    location: resourcesLocation 
    bundleName: productName
    nameEnding: commonNameEnding
    skuName: appPlanSku
    logAnalyticsResourceId: sharedResources.outputs.logAnalyticsWorkspaceId
    configStorageName: sharedResources.outputs.configStorageName
    configStorageRG: rgName
    dockerTag: dockerTag
    configureStaticContent: true
    instanceNum: gatewayInstanceNum
  }
  dependsOn: [
    sharedResources
  ]
}

output gateway object = {
  appServiceName: gatewayService.outputs.appServiceName
  hostEndpoint: gatewayService.outputs.hostEndpoint
  blobs: gatewayService.outputs.blobs
}

output rgName string = rgName
output configStorageName string = sharedResources.outputs.configStorageName
