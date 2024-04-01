# Global variables configuration

There are 3 ways to define global variables.

## 1. Separate global configuration json file
Global configuration file can be read from file system or Azure Storage Account. Expected scenario is to have different files for the different environments (i.e. dev, staging, production) and upload global configuration file to the storage account as part of deployment processes.

Below example defines ```remoteServerPath``` and ```anotherRemoteServerPath``` variables
```json5
{
  "variables": {
    "remoteServerPath": "https://dev.my-protected-server.com/api",
    "anotherRemoteServerPath": "https://dev.my-another-protected-server.com/api"
  }
}
```

## 2. Environment variables
Environment variables is another recommended approach to define global variables, if number of variables is not to large. To define global variable via environment variable, use ```Globals__Variables__``` prefix. So, as alternative of above configuration use:
```
set Globals__Variables__remoteServerPath="https://dev.my-protected-server.com/api"
set Globals__Variables__anotherRemoteServerPath="https://dev.my-another-protected-server.com/api"
```

## 3. Application configuration
Read more about app configuration [here](./ApplicationConfiguration.md)

Can be used for custom build of SecuredAPI as a docker image based on official image from docker hub. For if SecureAPI build from source code.

The equivalent of above settings define in appsettings.json:
```json5
{
  "Globals": {
    "Variables": {
      "remoteServerPath": "https://dev.my-protected-server.com/api",
      "anotherRemoteServerPath": "https://dev.my-another-protected-server.com/api"
    }
  }
}
```