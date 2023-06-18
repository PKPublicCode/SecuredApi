### Deploy performance testing environment
More about tests purpose and results you can see [here](./../Product/Performance.md)
To avoid conflicts, all resources created with specific name ending. You can set it using environment variable:

```
$env:SECURED_API_NAME_ENDING=<your ending>
```

All scripts below disigned to be executed in the same powershell console session. Deploy.ps1 sets various environment variables that required for execution of next scripts, including resource group names, service URLs, storage account names, etc. Also it allows re-deploy into the same environment.

As a result 2 resource groups will be created:
#####rg-secureapi-shared-<your ending>
Contains storage account to store routing configuration and api subscriptions, log analitics workspace instance
#####rg-secureapi-<your ending>
Contains 2 app services, 2 appservice plans, 2 app insights connected to log analitics workspace

#### Go to performance testing folder
```
cd ./Testing/PerformanceTests/
```
#### Deploy infra

Deploy infrastructure, use preset (with environment variable) name ending, Standard S1 sku plan and use latest build from docker:
```
./Deploy.ps1
```

Alternatively, you can use -Force flag to generate random ending if not set. Name ending will be saved in appropriate environment variable and reused in future if run script from the same session
```
./Deploy.ps1 -Force
```

Alternantively, you can deploy infra for DOTNET app using -DoNotDeployDocker and then deploy local build.
```
./Deploy.ps1 -DoNotDeployDocker
./PublishAppsFromLocal.ps1 #Build dotnet, zip and deploy web apps
```

Alternatively, app service plan SKU can be overriden with -AppPlanSku flag
```
./Deploy.ps1 -AppPlanSku "P1V3" 
```

#### Upload configuration
Below scripts deploy and configure app services to use routing configuration from storage account and Consumers\subscriptions stored in storage tables. 
To upload routing configurations run
```
./UploadConfiguration.ps1
```
__Note__: Current logged user will be assigned to Blob Owner Role
There are no cmd tools that can help with uploading storage tables. So, need manually upload to appropriate storage tables from foloder 'SubscriptionsMgmtTables'. Will be fixed later.
#### Smoke tests
Update hosts in SmokeTest.http according to you name ending. Then using [VS Code Rest Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) test echo and gateway endpoints
#### Jmeter tests
Tests designed as simple call rest call protected by API Key. Calls use same API Key but random path ending in URL to reduce possible effects of cachning.
Open ./Scenarios/Tests.jmx in jmeter app, update variables to use appropriate backend hosts (according to your name ending). 
#### Cleanup
Delete services but preserver shared resource group:
```
./Cleanup.ps1
```
Delete all:
```
./Cleanup.ps1 -DeleteShared
```
