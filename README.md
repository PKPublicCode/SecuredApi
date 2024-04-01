# SecuredApi

Simple (in terms of usage), cheap (no hard dependency on expensive services and solutions), PaaS compatible and code-first friendly gateway solution to offload routine web api operation for solutions with low and medium loaded web applications.

__Currently, main focus is compatibility on Azure. Compatibility with other clouds and 3-rd party services will be coming later.__

![](./Docs/Img/birdseye.png)

The main scenario for this solution is an API Gateway with a rich configuration that defines routing to downstream backend services for incoming HTTP(s) requests and allows to:
* Configure authentication and authorization rules using:
    * API Keys and customer subscriptions
    * JWT tokens and and claims (Azure Entra Only)
* Add and\or remove HTTP headers in request before sending to the downstream services
* Add and\or remove HTTP headers in response before returning it to client
* Restrict clients by inbound IP address
* Serve content, stored as blobs or as files on file system
* Return content defined in configuration inline
* Configuring any of above for individual customers (API Keys authentication only).

As a side scenario, solution can be useful to host mock and stub services for integration testing, and hosting static content on the Azure Storage Accounts.

SecuredAPI is available as a docker [images](https://hub.docker.com/repository/docker/pkruglov/securedapi.gateway) and routing configuration is decoupled from the build. This approach makes it easy to use with Azure Application Services and Kubernetes solutions, where configuration can be updated aside of deployment process.

However, service can be configured to read routing configuration from the filesystem, and application can build as own docker image based on the official SecuredAPI build to make all configuration part of the image. Obviously, own binaries with appropriate configuration can be built from source the code.

Integration with Application Insights available out of the box can be leveraged for monitoring and further analysis.

## Performance
Check out load testing [results](./Docs/Product/Performance.md). 

## License
This program is free software: you can redistribute it and/or modify it under the terms of the [Server Side Public License, version 1](./LICENSE.txt)

License explanation can be found [here](https://www.mongodb.com/licensing/server-side-public-license/faq)

License copied from [MongoDB github](https://github.com/mongodb/mongo/blob/master/LICENSE-Community.txt)

### Documentation
[Performance](./Docs/Product/Performance.md)
[Configuration](./Docs/Product/Configuration.md)

