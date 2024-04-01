# SecuredAPI configuration summary

The main usage scenario is deploying official SecureAPI images from the docker hub. That's why configuration mechanism supports configuring application without interfering with build artifacts and keeping configuration aside of the docker image.

SecuredAPI configuration contains of three main parts.
1. [Application configuration](./Configuration.md). Configures application services and infrastructure integration details. It can be defined either with environment variables, or with appsettings.json file.

2. [Global variables](./GlobalVariablesConfiguration.md). Defines parameters for routing configuration.

3. [Routing configuration](./RoutingConfiguration.md). Vital part of the SecuredAPI. It defines behavior of the Gateway and how requests are processed.