param bundleName string
param nameEnding string
param location string = resourceGroup().location
param appServicePlanName string = makeResourceName('plan', bundleName, nameEnding)
param webSiteName string = makeResourceName('app', bundleName, nameEnding)
param appInsightName string = makeResourceName('appi', bundleName, nameEnding)
param logAnalyticsResourceId string
param skuName string
param instanceNum int = 1
param httpsOnly bool = true
param dockerTag string = 'latest'
param dockerRegistyUrl string = 'https://index.docker.io/v1/'
param appServiceConfiguration object = {
}

param configStorageName string
param configStorageRG string
param configContainer string = makeContainerName('config', bundleName)
param configureStaticContent bool = false
param staticContentContainer string = makeContainerName('static', bundleName)

param configureSubscriptions bool = false
param subscriptionsContainer string = makeContainerName('subscriptions', bundleName)
param subscriptionKeysContainer string = makeContainerName('keys', bundleName)
param subscriptionKeysSalt string = ''
param configureConsumers bool = false
param consumersContainer string = makeContainerName('consumers', bundleName)

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightName
  location: location
  kind: 'string'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalyticsResourceId
  }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2020-06-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: skuName
    capacity: instanceNum
  }
  properties:{
    reserved: true
  }
}

var _deployFromDocker = !empty(dockerTag)
//see possible runtimes with "az webapp list-runtimes --os-type linux"
var _linuxFxVersion = _deployFromDocker ? 'DOCKER|pkruglov/securedapi.gateway:${dockerTag}' : 'DOTNETCORE|7.0'

// ########## Make app service settings
var _emptySettings = {}

var _dockerRegistrySettings = !_deployFromDocker ? _emptySettings : {
  DOCKER_REGISTRY_SERVER_URL: dockerRegistyUrl
}

//ToDo: 
// 1) Revise if all containers needed; 
var _subscriptionSettings = !configureSubscriptions ? _emptySettings : {
  Subscriptions__Keys__FileAccess__Type: 'AzureStorage'
  Subscriptions__Keys__FileAccess__Rbac__Uri: storageContent.outputs.blobUrls[subscriptionKeysContainer]
  Subscriptions__Keys__Security__Salt: subscriptionKeysSalt
  Subscriptions__Subscriptions__FileAccess__Type: 'AzureStorage'
  Subscriptions__Subscriptions__FileAccess__Rbac__Uri: storageContent.outputs.blobUrls[subscriptionsContainer]
}

var _consumersSettings = !configureConsumers ? _emptySettings : {
  Consumers__FileAccess__Type: 'AzureStorage'
  Consumers__FileAccess__Rbac__Uri: storageContent.outputs.blobUrls[consumersContainer]
}

var _routingSettings = {
  RoutingEngine__Files__RoutingCfgFileId: 'routing-config.json'
  RoutingEngine__FileAccess__Type: 'AzureStorage'
  RoutingEngine__FileAccess__Rbac__Uri: storageContent.outputs.blobUrls[configContainer]
}

var _staticContentSettings = !configureStaticContent ? _emptySettings : {
  StaticContent__FileAccess__Type: 'AzureStorage'
  StaticContent__FileAccess__Rbac__Uri: storageContent.outputs.blobUrls[staticContentContainer]
}

resource appService 'Microsoft.Web/sites@2022-09-01' = {
  name: webSiteName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: httpsOnly
    clientAffinityEnabled: false
    siteConfig: {
      minTlsVersion: '1.2'
      linuxFxVersion: _linuxFxVersion
      alwaysOn: true
    }
  }

  resource appServiceConfig 'config' = {
    name: 'appsettings'
    properties: union(
      _dockerRegistrySettings,
      _subscriptionSettings,
      _consumersSettings,
      _routingSettings,
      _staticContentSettings,
      appServiceConfiguration,
      { // Required settings
        APPINSIGHTS_INSTRUMENTATIONKEY: appInsights.properties.InstrumentationKey
        APPINSIGHTS_PROFILERFEATURE_VERSION: '1.0.0'
        APPINSIGHTS_SNAPSHOTFEATURE_VERSION: 'disabled'
        APPLICATIONINSIGHTS_CONNECTION_STRING: appInsights.properties.ConnectionString
        ApplicationInsightsAgent_EXTENSION_VERSION: '~2'
        DiagnosticServices_EXTENSION_VERSION: '~3'
        InstrumentationEngine_EXTENSION_VERSION: 'disabled'
        SnapshotDebugger_EXTENSION_VERSION: 'disabled'
        XDT_MicrosoftApplicationInsights_BaseExtensions: 'disabled'
        XDT_MicrosoftApplicationInsights_Mode: 'recommended'
        XDT_MicrosoftApplicationInsights_PreemptSdk: '1'
    })
  }

  resource appServiceAppSettings 'config' = {
    name: 'logs'
    properties: {
      applicationLogs: {
        fileSystem: {
          level: 'Warning'
        }
      }
      httpLogs: {
        fileSystem: {
          retentionInMb: 40
          enabled: true
        }
      }
      failedRequestsTracing: {
        enabled: true
      }
      detailedErrorMessages: {
        enabled: true
      }
    }
  }
}

//var configContainer = toLower('${bundleName}-config')
//If storage account is in another RG, it needs be specified it as scope and bicep compiler
//doesn't let to do it and asks to create separate module. That's why module is used
module storageAccess 'storage-access.bicep' = {
  name: '${bundleName}-config-container-access'
  scope: resourceGroup(configStorageRG)
  params: {
    storageName: configStorageName
    roles: ['blobReader']
    principalId: appService.identity.principalId
  }
}

module storageContent 'storage-content.bicep' = {
  name: '${bundleName}-config-container-content'
  scope: resourceGroup(configStorageRG)
  params: {
    storageName: configStorageName
    containers: union(
      [configContainer, staticContentContainer],
      configureConsumers ? [consumersContainer] : [],
      configureSubscriptions ? [subscriptionKeysContainer, subscriptionsContainer] : []
    )
  }
}

func makeContainerName(suffix string, bundleName string) string /*
*/  => toLower('${bundleName}-${suffix}')

func makeResourceName(prefix string, bundleName string, nameEnding string) string /*
*/  => toLower('${prefix}-${bundleName}-${nameEnding}')

output appServiceName string = appService.name
output hostEndpoint string = appService.properties.hostNames[0]

output blobs object = {
  configuration: {
    name: configContainer
    url: storageContent.outputs.blobUrls[configContainer]
  }
  staticContent: {
    name: staticContentContainer
    url: storageContent.outputs.blobUrls[staticContentContainer]
  }
  subscriptions: !configureSubscriptions ? null : {
    name: subscriptionsContainer
    url: storageContent.outputs.blobUrls[subscriptionsContainer]
  }
  subscriptionKeys: !configureSubscriptions ? null : {
    name: subscriptionKeysContainer
    url: storageContent.outputs.blobUrls[subscriptionKeysContainer]
  }
  consumers: !configureConsumers ? null : {
    name: consumersContainer
    url: storageContent.outputs.blobUrls[consumersContainer]
  }
}
