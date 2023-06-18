param bundleName string
param nameEnding string
param location string = resourceGroup().location
param logAnalyticsResourceId string
param skuName string
param skuCapacity int = 1
param configStorageName string
param configStorageRG string
param httpsOnly bool = true
param deployLatestFromDocker bool
param configureSubscriptionManagement bool = false
param appServiceConfiguration object = {
}

var appName = '${bundleName}-${nameEnding}'
var appServicePlanName = toLower('plan-${appName}')
var webSiteName = toLower('app-${appName}')
var appInsightName = toLower('appi-${appName}')

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightName
  location: location
  kind: 'string'
  tags: {
    displayName: 'AppInsight'
    ProjectName: appName
  }
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
  tags: {
    displayName: 'HostingPlan'
    ProjectName: appName
  }
  properties:{
    reserved: true
  }
}

//see possible runtimes with "az webapp list-runtimes --os-type linux"
var adjustedLinuxFxVersion = deployLatestFromDocker ? 'DOCKER|pkruglov/securedapi.gateway:latest' : 'DOTNETCORE|7.0'

var dockerRegistrySettings = {
  DOCKER_REGISTRY_SERVER_URL: 'https://index.docker.io/v1/'
}

// Adding docker settings to appSettings configuration provided as parameter
var withDockerSettings = deployLatestFromDocker /*
*/            ? union(appServiceConfiguration, dockerRegistrySettings) /*
*/            : appServiceConfiguration

var consumersTable = '${bundleName}Consumers'
var subscriptionsTable = '${bundleName}Subscriptions'
var subscriptionKeysTable = '${bundleName}SubscriptionKeys'

var subscriptionManagementnCfgSettings = {
  Consumers__Repository__Rbac__TableName: consumersTable
  Consumers__Repository__Rbac__Endpoint: storageConfiguration.outputs.tableEndpoint
  Subscriptions__Repository__Rbac__TableName: subscriptionsTable
  Subscriptions__Repository__Rbac__Endpoint: storageConfiguration.outputs.tableEndpoint
  SubscriptionKeys__Repository__Rbac__TableName: subscriptionKeysTable
  SubscriptionKeys__Repository__Rbac__Endpoint: storageConfiguration.outputs.tableEndpoint
}

// Adding subscription management 
var withSubscriptionMgmtSettings = configureSubscriptionManagement /*
*/            ? union(withDockerSettings, subscriptionManagementnCfgSettings) /*
*/            : withDockerSettings

resource appService 'Microsoft.Web/sites@2020-06-01' = {
  name: webSiteName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  tags: {
    displayName: 'Website'
    ProjectName: appName
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: httpsOnly
    siteConfig: {
      minTlsVersion: '1.2'
      linuxFxVersion: adjustedLinuxFxVersion
    }
  }

  resource appServiceConfig 'config' = {
    name: 'appsettings'
    properties: union(withSubscriptionMgmtSettings, { //combine common and conditional app settings
//App insights configuration
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
//### App specific configuration
//######## Routing configuration
      RoutingEngineManager__Files__RoutingCfgFileId: 'routing-config.json'
      RoutingEngineManager__FileAccess__Type: 'AzureStorage'
      RoutingEngineManager__FileAccess__Rbac__Uri: storageConfiguration.outputs.blobUrls[0]
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


var storageTables = configureSubscriptionManagement /*
*/          ? [consumersTable, subscriptionsTable, subscriptionKeysTable] /*
*/          : []

var storageRoles = configureSubscriptionManagement /*
*/          ? ['blobReader', 'tableReader'] /*
*/          : ['blobReader']

var configContainer = toLower('${bundleName}-config')
//If storage account is in another RG, it needs be specified it as scope and bicep compiler
//doesn't let to do it and asks to create separate module. That's why module is used
module storageConfiguration 'storage-configuration.bicep' = {
  name: '${bundleName}-config-container'
  scope: resourceGroup(configStorageRG)
  params: {
    storageName: configStorageName
    containers: [
      configContainer
    ]
    tables: storageTables
    roles: storageRoles
    principalId: appService.identity.principalId
  }
}

output appServiceName string = appService.name
output hostEndpoint string = appService.properties.hostNames[0]
output configBlobContainerUrl string = storageConfiguration.outputs.blobUrls[0]
output configBlobContainernName string = configContainer
output tableEndpoint string = configureSubscriptionManagement ? storageConfiguration.outputs.tableEndpoint : 'N/A'
output consumersTable string = configureSubscriptionManagement ? consumersTable : 'N/A'
output subscriptionsTable string = configureSubscriptionManagement ? subscriptionsTable : 'N/A'
output subscriptionKeysTable string = configureSubscriptionManagement ? subscriptionKeysTable : 'N/A'
