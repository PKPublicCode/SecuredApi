targetScope='subscription'

param location string = 'westeurope'
param servicesRgName string = 'secureapi'
param sharedRgName string = 'secureapi-shared'
param commonNameEnding string
param appPlanSku string = 'S1'
param gatewayInstanceNum int = 1
param deployLatestFromDocker bool = true


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
    deployLatestFromDocker: deployLatestFromDocker
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
    deployLatestFromDocker: deployLatestFromDocker
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

output sharedRgName string = sharedRG.name
output configStorageName string = sharedResources.outputs.configStorageName
output performanceTestRgName string = performanceTestsRG.name
