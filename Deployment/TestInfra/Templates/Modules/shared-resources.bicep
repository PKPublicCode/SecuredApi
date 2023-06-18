param baseName string = 'secureapi-shared'
param nameEnding string
param location string = resourceGroup().location
param logAnalyticsSku string = 'PerGB2018'
param logAnalyticsRetentionInDays int = 30
@minLength(3)
@maxLength(6)
param storageShortName string = 'cfg'
param storageSkuName string = 'Standard_LRS'

var logAnaliticsName = toLower('law-${baseName}-${nameEnding}')
var storageName = toLower('st${storageShortName}${replace(nameEnding, '-', '')}')

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2021-12-01-preview' = {
  name: logAnaliticsName
  location: location
  properties: {
    sku: {
      name: logAnalyticsSku
    }
    retentionInDays: logAnalyticsRetentionInDays
  }
}

resource storage 'Microsoft.Storage/storageAccounts@2021-09-01' = {
  name: storageName
  location: location
  sku: {
    name: storageSkuName
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
    allowBlobPublicAccess: false
    allowCrossTenantReplication: false
    allowSharedKeyAccess: false
    encryption: {
      keySource: 'Microsoft.Storage'
      services: {
        blob: {
          enabled: true
          keyType: 'Account'
        }
        file: {
          enabled: true
          keyType: 'Account'
        }
        queue: {
          enabled: true
          keyType: 'Service'
        }
        table: {
          enabled: true
          keyType: 'Service'
        }
      }
    }
 
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
  }
}

output logAnalyticsWorkspaceId string = logAnalyticsWorkspace.id
output configStorageName string = storage.name
