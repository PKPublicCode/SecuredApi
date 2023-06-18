param storageName string
param containers array = []
param tables array = []
@allowed([
  'blobReader'
  'blobContributor'
  'tableReader'
  'tableContributor'
])
param roles array = []
param principalId string = ''

// Creating a symbolic name for an existing resource
resource storage 'Microsoft.Storage/storageAccounts@2021-02-01' existing = {
  name: storageName
}

var storagePrefix = toLower('${storage.name}/default/')

resource container 'Microsoft.Storage/storageAccounts/blobServices/containers@2022-05-01' = [for container in containers: {
  name: toLower('${storagePrefix}${container}')
  properties: {
    publicAccess: 'None'
    metadata: {}
  }
}]

resource table 'Microsoft.Storage/storageAccounts/tableServices/tables@2022-09-01' = [for table in tables: {
  name: '${storagePrefix}/${table}'
}]

var roleDefinitions = {
  blobReader: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '2a2b9908-6ea1-4ae2-8e65-a410df84e7d1')
  blobContributor: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'ba92f5b4-2d11-453d-a403-e96b0029c9fe')
  tableReader: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '76199698-9eea-4c19-bc75-cec21354c6b6')
  tableContributor: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '0a9a7e1f-b9d0-4cc4-a60d-0319b160aaa3')
}

var resultingRoles = empty(principalId) ? [] : roles 

resource roleReaderAuthorization 'Microsoft.Authorization/roleAssignments@2020-10-01-preview' = [for role in resultingRoles: {
  // Generate a unique but deterministic resource name
  name: guid('storage-rbac', storage.id, resourceGroup().id, principalId, roleDefinitions[role])
  scope: storage
  properties: {
      principalId: principalId
      roleDefinitionId: roleDefinitions[role]
      principalType: 'ServicePrincipal'
  }
}]

output blobUrls array = [for c in containers: '${storage.properties.primaryEndpoints.blob}${c}' ]
output tableEndpoint string = storage.properties.primaryEndpoints.table
