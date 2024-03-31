# SecuredApi

Simple (in terms of usage), cheap (in terms of dependency to expensive services), code-first friendly and (Azure) PaaS compatible gateway solution to offload routine web api operation.

![Diagram](./Docs/Img/birdseye.png)

Rich configuration defines routing to downstream backend services for incoming HTTP(s) requests and allows to:
* Configure authentication and authorization rules using:
    * API Keys and customer subscriptions
    * JWT tokens and and claims (Azure Entra Only)
* Add and\or remove HTTP headers in request before sending to backend
* Add and\or remove HTTP headers in response before returning it to client
* Restrict clients by inbound IP address
* Return content, stored as blobs or as files on file system
* Return content defined in configuration inline

## License
This program is free software: you can redistribute it and/or modify it under the terms of the [Server Side Public License, version 1](./LICENSE.txt)

License explanation can be found [here](https://www.mongodb.com/licensing/server-side-public-license/faq)

License copied from [MongoDB github](https://github.com/mongodb/mongo/blob/master/LICENSE-Community.txt)

## Motivation
This exercise is inspired by experience working on SOA solution. Solution was implemented by small dev team (without DevOps), hosted in Azure (PaaS) with reasonably limited budget for hosting. Product provided API to 3-rd party services with api key authentication, in addition had oath authorization for user interaction. One of the challange was to find simple and affordable solution that can help us offload basic api management operations. This scenario is used as a backbone of the product.

Goal: Implement simple (in terms of usage), cheap (no hard dependency to expencive cloud solutions) and PaaS friendly (can be maintained without dedicated devops) solution that can offload basic API management operations from backend services. 

Current focus is on Azure compatibility, however keeping in mind migration to other clouds.

### Alternatives
#### Azure API Management
 - Pricing tiers. VNET integration available only in Primium tier, that costs an arm and a leg. Without VNET integration, integration of APIM to new solution is tricky. Integration into complex existing solution, that leverages Kubernetes, PaaS, deployed into multiple regions could be very painful.
 - Unfortunate setup and configuration of Subscriptions and Products:
    Subscription -> API
    Subscription -> Product -> APIs
    Where subscription is not configurable (e.g. can't setup whitelisting on Subscription (consumer) level)

## Features
* Routing
* Execute configurable actions on REST Calls
* Run consumer and subscriptionn specific actions
* API Key authentication

### Documentation
[Performance](./Docs/Product/Performance.md)
[Development](./Docs/Development)

