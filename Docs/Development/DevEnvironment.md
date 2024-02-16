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

##### needed for integration tests and for few ps scripts
```
export SECURED_API_INTEGRATION_TESTS__SpiClient__ClientId=""
export SECURED_API_INTEGRATION_TESTS__SpiClient__ClientSecret=""
export SECURED_API_INTEGRATION_TESTS__SpiClient__TenantId=""
export SECURED_API_INTEGRATION_TESTS__SpiClient__Scope=""

export Globals__Variables__AllowedEntraTokenIssuer=""
export Globals__Variables__AllowedEntraTokenAudience=""
```

##### Not used anymore
```
export SecuredApi__Global__Keyvault=https://kv-name.vault.azure.net/
```

If above vars set in .bash_profile, then to make VS for Mac accept above variables, app should be launced from console
``` open /Applications/Visual\ Studio.app ```
