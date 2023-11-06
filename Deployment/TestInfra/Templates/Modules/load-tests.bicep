param bundleName string
param nameEnding string
param location string = resourceGroup().location
param name string = toLower('lt-${bundleName}-${nameEnding}')

resource loadTests 'Microsoft.LoadTestService/loadTests@2022-12-01' = {
  location: location
  name: name
}

output resourceName string = name
