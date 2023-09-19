param storageName string
param containers array = []
param tables array = []

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

//ToDo. will not work, fix
// https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/bicep-functions-lambda
// https://github.com/Azure/bicep/issues/8601
output blobUrls array = [for c in containers: '${storage.properties.primaryEndpoints.blob}${c}' ]
