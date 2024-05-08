WIP

## Local testing

#### To run tests and deploy env you need following variables:
##### short code that will be used as ending of resources names during during deployment 
```
export SECURED_API_NAME_ENDING=code
```

##### needed for integration tests, deployment scripts and for few ps experimental scripts (ex. to aquire access tokens)
```
export SECAPI_IT_MAIN__SpiClient__TenantId=""
export SECAPI_IT_MAIN__SpiClient__ClientId=""
export SECAPI_IT_MAIN__SpiClient__ClientSecret=""
export SECAPI_IT_MAIN__SpiClient__Scope=""

export SECAPI_IT_GW__Globals__Variables__AllowedEntraTokenIssuer=""
export SECAPI_IT_GW__Globals__Variables__AllowedEntraTokenAudience=""

export SECAPI_IT_GW__Subscriptions__Keys__FileAccess__Rbac__Uri=""
export SECAPI_IT_GW__Consumers__FileAccess__Rbac__Uri=""
export SECAPI_IT_GW__RoutingEngine__FileAccess__Rbac__Uri=""
export SECAPI_IT_GW__StaticFilesProvider__FileAccess__Rbac__Uri=""

```

##### For local run of jmeter tests:
```
export SRV_URL_PATH="/api/jwt/basic_features/delay"
export SRV_URL=""
```

If above vars set in .bash_profile, then to make VS for Mac accept above variables, app should be launched from console
``` open /Applications/Visual\ Studio.app ```
