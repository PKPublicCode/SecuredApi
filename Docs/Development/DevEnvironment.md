WIP!!!!

# Tools and Depencies
Below tools user for development and build
#### Powershell
``` 
brew install --cask powershell 
pwsh #test run
```
Required modules
```
Install-Module -Name Az -Repository PSGallery -Forces
Install-Module AzTable # Not used anymore
Install-Module -Name MSAL.PS
```
#### Jmeter
Install:
Run:
```
open /usr/local/bin/jmeter
```
#### Azure CLI
#### Manual Install bicep
```
# Add the tap for bicep
brew tap azure/bicep

# Install the tool
brew install bicep
```

#### Docker
According to your choice

#### To run tests and deploy env you need following variables:
##### short code that will be used as ending of resources during 
```
export SECURED_API_NAME_ENDING=code
```

##### needed for integration tests and for few ps experimental scripts (ex. to aquire access tokens)
```
export SECAPI_IT_MAIN__SpiClient__TenantId=""
export SECAPI_IT_MAIN__SpiClient__ClientId=""
export SECAPI_IT_MAIN__SpiClient__ClientSecret=""
export SECAPI_IT_MAIN__SpiClient__Scope=""

export SECAPI_IT_GW__Globals__Variables__AllowedEntraTokenIssuer=""
export SECAPI_IT_GW__Globals__Variables__AllowedEntraTokenAudience=""
export SECAPI_IT_GW__Subscriptions__Keys__FileAccess__Rbac__Uri=""
export SECAPI_IT_GW__Subscriptions__Consumers__FileAccess__Rbac__Uri=""
export SECAPI_IT_GW__RoutingEngineManager__FileAccess__Rbac__Uri=""

```

If above vars set in .bash_profile, then to make VS for Mac accept above variables, app should be launced from console
``` open /Applications/Visual\ Studio.app ```
