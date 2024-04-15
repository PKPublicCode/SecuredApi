# Application configuration

## Basics
Configures SecuredAPI components and access to the infrastructure using conventional asp.net approaches. SecuredAPI uses jsonfile (appsettings.json) and environment variables configuration providers. The main scenario supposes deploying SecuredAPI as docker image from docker hub, and everything is configured with environment variables. However, for the sake of simplification, configuration will be described in json format.

Detailed description how to configure asp.net apps is [here](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-8.0#non-prefixed-environment-variables). Frankly speaking, to mimic nested configuration, __ separator is used in the variable name. Following json and environment variables defines equivalent configuration.

```json5
"Position": {
    "Title": "Editor",
    "Name": "Joe Smith"
  }
```

```
set Position__Title=Environment_Editor
set Position__Name=Environment_Rick
```

# TBD