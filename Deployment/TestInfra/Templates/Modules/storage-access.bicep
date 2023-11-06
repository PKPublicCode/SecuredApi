param storageName string
@allowed([
  'blobReader'
  'blobContributor'
  'tableReader'
  'tableContributor'
])
param roles array
param principalId string

// Creating a symbolic name for an existing resource
resource storage 'Microsoft.Storage/storageAccounts@2021-02-01' existing = {
  name: storageName
}

var _roleDefinitions = {
  blobReader: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '2a2b9908-6ea1-4ae2-8e65-a410df84e7d1')
  blobContributor: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'ba92f5b4-2d11-453d-a403-e96b0029c9fe')
  tableReader: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '76199698-9eea-4c19-bc75-cec21354c6b6')
  tableContributor: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '0a9a7e1f-b9d0-4cc4-a60d-0319b160aaa3')
}

var _resultingRoles = empty(principalId) ? [] : roles 

resource roleReaderAuthorization 'Microsoft.Authorization/roleAssignments@2020-10-01-preview' = [for role in _resultingRoles: {
  // Generate a unique but deterministic resource name
  name: guid('storage-rbac', storage.id, resourceGroup().id, principalId, _roleDefinitions[role])
  scope: storage
  properties: {
      principalId: principalId
      roleDefinitionId: _roleDefinitions[role]
      principalType: 'ServicePrincipal'
  }
}]
