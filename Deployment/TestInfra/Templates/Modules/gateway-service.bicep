param bundleName string
param nameEnding string
param location string = resourceGroup().location
param appServicePlanName string = makeResourceName('plan', bundleName, nameEnding)
param webSiteName string = makeResourceName('app', bundleName, nameEnding)
param appInsightName string = makeResourceName('appi', bundleName, nameEnding)
param logAnalyticsResourceId string
param skuName string
param skuCapacity int = 1
param httpsOnly bool = true
param deployLatestFromDocker bool
param dockerRegistyUrl string = 'https://index.docker.io/v1/'
param appServiceConfiguration object = {
}

param configStorageName string
param configStorageRG string
param configContainer string = makeContainerName('config', bundleName)

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
    capacity: skuCapacity
  }
  properties:{
    reserved: true
  }
}

//see possible runtimes with "az webapp list-runtimes --os-type linux"
var _linuxFxVersion = deployLatestFromDocker ? 'DOCKER|pkruglov/securedapi.gateway:latest' : 'DOTNETCORE|7.0'

// ########## Make app service settings
var _emptySettings = {}

var _dockerRegistrySettings = !deployLatestFromDocker ? _emptySettings : {
  DOCKER_REGISTRY_SERVER_URL: dockerRegistyUrl
}

//ToDo: 
// 1) Revise all containers needed; 
// 2) Fix blobs
var _subscriptionSettings = !configureSubscriptions ? _emptySettings : {
  Subscriptions__Keys__FileAccess__Type: 'AzureStorage'
  Subscriptions__Keys__Rbac__Uri: storageContent.outputs.blobUrls[1]
  Subscriptions__Keys__Security__Salt: subscriptionKeysSalt
  Subscriptions__Subscriptions__FileAccess__Type: 'AzureStorage'
  Subscriptions__Subscriptions__Rbac__Uri: storageContent.outputs.blobUrls[2]
}

var _consumersSettings = !configureConsumers ? _emptySettings : {
  Subscriptions__Consumers__FileAccess__Type: 'AzureStorage'
  Subscriptions__Consumers__Rbac__Uri: storageContent.outputs.blobUrls[3]
}

var _routingSettings = {
  RoutingEngineManager__Files__RoutingCfgFileId: 'routing-config.json'
  RoutingEngineManager__FileAccess__Type: 'AzureStorage'
  RoutingEngineManager__FileAccess__Rbac__Uri: storageContent.outputs.blobUrls[0]
}

resource appService 'Microsoft.Web/sites@2020-06-01' = {
  name: webSiteName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: httpsOnly
    siteConfig: {
      minTlsVersion: '1.2'
      linuxFxVersion: _linuxFxVersion
    }
  }

  resource appServiceConfig 'config' = {
    name: 'appsettings'
    properties: union(
      _dockerRegistrySettings,
      _subscriptionSettings,
      _consumersSettings,
      _routingSettings,
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
  name: '${bundleName}-config-container'
  scope: resourceGroup(configStorageRG)
  params: {
    storageName: configStorageName
    roles: ['blobReader']
    principalId: appService.identity.principalId
  }
}

module storageContent 'storage-content.bicep' = {
  name: '${bundleName}-config-container'
  scope: resourceGroup(configStorageRG)
  params: {
    storageName: configStorageName
    containers: union(
      [configContainer],
      configureConsumers ? [consumersContainer] : [],
      configureSubscriptions ? [subscriptionKeysContainer, subscriptionsContainer] : []
    )
  }
}

func makeContainerName(bundleName string, suffix string) string /*
*/  => toLower('${bundleName}-${suffix}')

func makeResourceName(prefix string, bundleName string, nameEnding string) string /*
*/  => toLower('${prefix}-${bundleName}-${nameEnding}')

output appServiceName string = appService.name
output hostEndpoint string = appService.properties.hostNames[0]
output configBlobContainerUrl string = storageContent.outputs.blobUrls[0]
output configBlobContainernName string = configContainer
//output tableEndpoint string = configureSubscriptionManagement ? storageConfiguration.outputs.tableEndpoint : 'N/A'
//output consumersTable string = configureSubscriptionManagement ? consumersTable : 'N/A'
//output subscriptionsTable string = configureSubscriptionManagement ? subscriptionsTable : 'N/A'
//output subscriptionKeysTable string = configureSubscriptionManagement ? subscriptionKeysTable : 'N/A'
