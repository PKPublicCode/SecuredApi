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

output blobUrls object = reduce(containers, {}, (res, curr) => union(res, { '${curr}' : '${storage.properties.primaryEndpoints.blob}${curr}'}))
