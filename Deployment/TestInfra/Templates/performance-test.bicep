targetScope='subscription'

param location string = 'westeurope'
param servicesRgName string = 'secureapi'
param sharedRgName string = 'secureapi-shared'
param commonNameEnding string
param appPlanSku string = 'S1'
param gatewayInstanceNum int = 1
param dockerTag string = 'latest'


resource sharedRG 'Microsoft.Resources/resourceGroups@2021-01-01' = {
  name: toLower('rg-${sharedRgName}-${commonNameEnding}')
  location: location
}

module sharedResources './Modules/shared-resources.bicep' = {
  name: 'shared-resources'
  scope: sharedRG
  params: {
    nameEnding: commonNameEnding
    storageShortName: 'cfg'
  }
}

resource performanceTestsRG 'Microsoft.Resources/resourceGroups@2021-01-01' = {
  name: toLower('rg-${servicesRgName}-${commonNameEnding}')
  location: location
}

module gatewayService './Modules/gateway-service.bicep' = {
  name: 'gateway-service'
  scope: performanceTestsRG
  params: {
    bundleName: 'ApiGateway'
    nameEnding: commonNameEnding
    skuName: appPlanSku
    logAnalyticsResourceId: sharedResources.outputs.logAnalyticsWorkspaceId
    configStorageName: sharedResources.outputs.configStorageName
    configStorageRG: sharedRG.name
    dockerTag: dockerTag
    configureSubscriptions: true
    subscriptionKeysSalt: '5b951d0869cc4d2da993b6d188197c71'
    configureConsumers: true
    instanceNum: gatewayInstanceNum
    appServiceConfiguration: {
      GlobalVariables__EchoPath: 'http://${echoService.outputs.hostEndpoint}/echo'
    }
  }
}

module echoService './Modules/gateway-service.bicep' = {
  name: 'echo-service'
  scope: performanceTestsRG
  params: {
    bundleName: 'EchoSrv'
    nameEnding: commonNameEnding
    skuName: appPlanSku
    httpsOnly: false
    logAnalyticsResourceId: sharedResources.outputs.logAnalyticsWorkspaceId
    configStorageName: sharedResources.outputs.configStorageName
    configStorageRG: sharedRG.name
    dockerTag: dockerTag
  }
}

//Can't create resource directly in this bicep because current scope is subscription. Have to use module
module loadTesting './Modules/load-tests.bicep' = {
  name: 'LoadTesting'
  scope: performanceTestsRG
  params: {
    bundleName: 'shared'
    nameEnding: commonNameEnding
  }
}

output gateway object = {
  appServiceName: gatewayService.outputs.appServiceName
  hostEndpoint: gatewayService.outputs.hostEndpoint
  blobs: gatewayService.outputs.blobs
}

output echo object = {
  appServiceName: echoService.outputs.appServiceName
  hostEndpoint: echoService.outputs.hostEndpoint
  blobs: echoService.outputs.blobs
}

output loadTesting object = {
  name: loadTesting.outputs.resourceName
}

output sharedRgName string = sharedRG.name
output configStorageName string = sharedResources.outputs.configStorageName
output performanceTestRgName string = performanceTestsRG.name
