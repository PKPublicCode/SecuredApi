targetScope='subscription'

param resourcesLocation string
param location string = resourcesLocation
param productName string
param commonNameEnding string
param rgName string = toLower('rg-${productName}-${commonNameEnding}')
param appPlanSku string = 'S1'
param gatewayInstanceNum int = 1
param dockerTag string = 'latest'

resource envRG 'Microsoft.Resources/resourceGroups@2021-01-01' = {
  name: rgName
  location: location
}

module sharedResources './Modules/shared-resources.bicep' = {
  name: 'shared-resources'
  scope: envRG
  params: {
    nameEnding: commonNameEnding
    storageShortName: 'cfg'
  }
}

module gatewayService './Modules/gateway-service.bicep' = {
  name: 'main-service'
  scope: envRG
  params: {
    bundleName: productName
    nameEnding: commonNameEnding
    skuName: appPlanSku
    logAnalyticsResourceId: sharedResources.outputs.logAnalyticsWorkspaceId
    configStorageName: sharedResources.outputs.configStorageName
    configStorageRG: envRG.name
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

output sharedRgName string = envRG.name
output configStorageName string = sharedResources.outputs.configStorageName
output performanceTestRgName string = envRG.name
