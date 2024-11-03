# Configuration
Before this article check out [How SecuredAPI works](./Details.md)

Routing configuration is defined in json format. Comments are allowed. Max depth is 64 (default for .net).

# Main configuration concepts
* [Routes Group](#routes-group-and-root-element)
* [Route](#route)
* [Action](#actions)
* [Variables](#variables)

The overall config structure can be depicted as following: 
```JSON5
{ // Root RoutesGroup
  "RoutesGroups": [ // Groups routes with commonalities like url path, auth rules, etc
    {
      "RoutesGroups": [ // Can be up to 30 nested groups
        {
          "Routes": [ // Routes for these groups, that is terminal element in the Route group structure
            {
              "Actions": [ // Actions executed for specific route
              ]
            }
          ]
        }
      ]
    }
  ]
}
```

## Routes Group and root element
Routes Group intended to define configuration as tree-like structure and allows to group your routes with commonalities, like common beginning of the url path, that have common auth rules, required to run same actions, etc. JSON object has following structure:

```JSON5
{
  "description": "Free text", // Optional. User friendly description for the group, for maintenance and troubleshooting purposes
  "id": "unique_guid", // Optional, used by api key (subscriptions) authorization, or for troubleshooting
  "path": "/api", // Optional, common url path beginning
  "preRequestActions": [ // Actions that will be executed before running nested group's preRequestActions and routes.actions
  ],
  "onRequestErrorActions": [ // Optional. Actions that will be executed after running nested group's onRequestErrorActions and routes' actions in case of error of nested routes
  ],
  "onRequestSuccessActions": [ // Optional. Actions that will be executed  after running nested group's onRequestSuccessActions and routes' actions in case of success of nested routes
  ],
  "routes": [ // Definition of specific rotes. Can't be used together with routesGroups property
  ],
  "routesGroups": [ // Array of nested groups of routes. Can't be used together with routes property
    /*{ }*/ // Here you can define nested grouping structure same route groups where you can setup 
  ]
}
```

Route groups has tree structure, meaning that one routes group can combine many nested routing groups, each of nested routes group can define other route groups and so on until the routes are defined. Routes group can't have both ```routes``` and ```routesGroups``` properties and one of these properties has to be set.

Root element has same format as ```RoutesGroup``` element with few restrictions and limitations:
* It can't have ```routes``` property set. Only ```routesGroups``` property can be used to configure nested routes
* It has property ```routeNotFoundActions``` to configure actions if request url was not found in routing configuration.

```JSON5
{
  "id": "unique_guid", // Optional, used by api key (subscriptions) authorization, or for troubleshooting
  "path": "/api", // Optional, common url path beginning
  "preRequestActions": [
  ],
  "onRequestErrorActions": [
  ],
  "onRequestSuccessActions": [
  ],
  "routesGroups": [ // Array of nested groups of routes. Can't be used together with routes property
    /*{ }*/ // Here you can define nested grouping structure same route groups where you can setup 
  ],
  "routeNotFoundActions": [ // Required. Defines actions that will be executed if route not found. 
    {
      "type": "SetResponse", // In this case it returns status code 404
      "httpCode": 404,
    }
  ]
}
```

## Route
Route defines the list of actions that will be executed for the specific url path and http method.

```JSON5
{
  "description": "Free text", // Optional. User friendly description for the route, for maintenance and troubleshooting purposes
  "id": "unique guid", // Optional, used for troubleshooting purposes
  "path": "/some_resource/", // Required, identifies the rest of route, allows using * (asterisk) to make route work for call with any ending.
  "methods": [ "get" ], // Required, and must have at least one element. HTTP Methods that identifies route together with url path
  "actions": [ // Required. Actions that will be executed for this route
  ]
}
```

Pair of path and http method is a identifier of the route. SecuredAPI allows to specify multiple methods, that can be treated a shortcut to define multiple routes with same path but different methods. 

```path``` property is cumulative, and is a concatenation of paths defined in the parent routes groups and defined in specific route. ```path``` property defines exact path, unless asterisk is used in the end (**and only in the end**). In case of asterisk, route is used for calls with any url path ending.

Consider below example:

```JSON5
{ 
  "RoutesGroups": [ 
    {
      "path": "/api",
      "RoutesGroups": [
        {
          "path": "/resources",
          "Routes": [ 
            {
              "description": "Requests for Resource A",
              "path": "/resource_a/*",
              "methods": [ "get", "post" ],
              "Actions": [
              ]
            },
            {
              "description": "Requests for Resource A",
              "path": "/resource_b",
              "methods": [ "put" ],
              "Actions": [ 
              ]
            }
          ]
        }
      ]
    }
  ]
}
```

This snipped defines 3 routes:
* ```get, /api/resources/resource_a/*```
* ```ost, /api/resources/resource_a/*```
* ```put, /api/resources/resource_b```

These routes address following requests:
* ```[get] /api/resources/resource_a/```
* ```[get] /api/resources/resource_a/resource_y```
* ```[post] /api/resources/resource_a/```
* ```[post] /api/resources/resource_a/resource_z```
* ```[put] /api/resources/resource_b/```

However routes for below requests will not be found: 

* ```[put] /api/resources/resource_b/resource_z``` - because route for Resource B url doesn't have asterisk and works only for exact path

* ```[put] /api/resources/resource_a/``` - because route for Resource A url defined only for get and post methods

## Actions
Actions describe transformation, validation and other procedures that executed for the route or group of routes. They are executed one by one in order described in section. Execution order of sections corresponds to the parentheses: 

[Root preRequestActions]->[Level 1 Routes Group preRequestActions]->[Level 2 Routes Group preRequestActions]->...->[Route's actions].

After that onRequestErrorActions or onRequestSuccessActions are executed in reversed order:
[Deepest routes group onRequestSuccessActions]->[One level up routes group onRequestSuccessActions]->...->[Root onRequestSuccessActions]

Action has only one common required filed ```type```. This field defines specific action. Other fields can be optional or mandatory depending on the actual action.

Full list of available actions are under construction.

See below example.

```json5
{
  /* ... */
  "routesGroups":[
    {
      "path":"/api",
      "preRequestActions":[
        {
          "type": "SetRequestHeader", //adds header into request before calling downstream service
          "name": "X-REQUEST-HEADER",
          "value": "Request came from SecuredAPI"
        }
      ],
      "onRequestSuccessActions": [
        {
          "type": "SetResponseHeader", //adds header in case of successful call of downstream service
          "name": "X-RESPONSE-HEADER",
          "value": "This request processed with SecuredAPI"
        },
      ],
      "routes": [ 
        {
          "description": "Requests for Resource A",
          "path": "/resource_a/",
          "methods": [ "get" ],
          "Actions": [
            {
              "type": "RemoteCall", // calls protected service
              "path": "https://my-private-server/resource_a",
              "method": "get"
            },
            {
              "type": "SuppressResponseHeaders", // removes below headers from the response before returning it to the client
              "headers": [ "X-MY-PRIVATE-SERVER-HEADER-1", "X-MY-PRIVATE-SERVER-HEADER-1" ]
            }
          ]
        }
      ]
    }
  ]
}
```

This example describes behaviour if gateway receives http call ```[get] /api/resource_a```:
* Request received from client is transformed by adding "X-REQUEST-HEADER"
* Downstream service called with ```[get] https://my-private-server/resource_a```
* Response headers are stripped and "X-MY-PRIVATE-SERVER-HEADER-1", "X-MY-PRIVATE-SERVER-HEADER-1" are removed
* Header with name "X-RESPONSE-HEADER" is added to response
* Request returned to the client

Find all currently supported actions [here](./actions.md)

## Variables and runtime expressions
Variables allow to parametrize routing configuration. There are two types of variables: Global and Runtime. All variable names are case sensitive and has to be alpha-numeric string started with letter.

### Global variables
Global variables are immutable parameters defined outside of the routing configuration by the gateway owner loaded before parsing and initialization of routing configuration. Main purpose is to use same routing configuration file for different deployment environments.

Format: ```${variableName}```. If variable is not found during routing configuration parsing and initialization, error is thrown and whole configuration is ignored.

Global variables can be used as part of any string value of the routing config file and substituted by the appropriate value during configuration initialization process, that makes zero performance impact during the executing request.

How to configure global variables see [here](./GlobalVariablesConfiguration.md)

### Runtime expressions and variables
In contrast to global variables, runtime expression are evaluated during request execution. It allows to use properties and values of the specific request.

Runtime expressions implemented as functions with zero or one parameter. Format: ```@{function(parameter)}```. For example ```@{getRequestMethod()}``` returns http method of currently executing incoming client request. While functions names are hardcoded and are known during configuration parsing, parameters could be dynamic. For example ```@{getQueryParam(param)}``` retrieves value of the specific parameter of the current request. This is why, validation of the runtime exceptions is partially happens in runtime. If routing configuration file uses unknown function, configuration parsing is stopped, however if parameter is not applicable, error (exception) happens during the request execution.

Some actions could cache values for further usage during request execution. For example ```CheckSubscription``` action stores ```ConsumerId``` value as runtime variable. This variable can be retrieved as ```@{getVariable(ConsumerId)}```


See list [of available function](./RuntimeFunctions.md).
See list [of available variables](./RuntimeVariables.md).

In below example, incoming request will be passed to the service defined by application owner as ```protectedEchoPath``` and can be different for different environments. Method of outgoing request is ```getRequestMethod()``` that means the same as request method. ```getRemainingPath()``` is a url path that corresponds to the asterisk and is remaining part after removing ```/resource_a/``` in the beginning.

```json5
"Routes": [ 
  {
    "description": "Requests for Resource A",
    "path": "/resource_a/*",
    "methods": [ "get", "post" ],
    "Actions": [
      {
        "type": "RemoteCall",
        "path": "$(remoteServerPath)/@{getRemainingPath()}", 
        "method": "@{getRequestMethod()}"
      }
    ]
  }
]
```

So, if global variable defined as ```"remoteServerPath": "http://myserver/api"```json5, and client sends request ```[get] /resource_a/resource_x/resource_y```, then outgoing request according to above configuration will be ```[get] http://myserver/api/resource_x/resource_y```

Only action properties with type ```RuntimeExpression``` can use runtime expressions.

# Examples
Gateway [configuration](../../Testing/CommonContent/Configuration/routing-config-gateway.json) used for integration tests. There are 3 routing groups in the config root that configures behavior as:
1) protects calls to ```/api/jwt/``` using Entra JWT Access token.
* First, the routing group verifies jwt's signature, allowed issuer and audience. Issuer and audience are parametrized using global variables
* For route ```/api/api_key/basic_features/*```, JWT is verified for role ```EchoSrv.API.Basic"```
* For route ```/api/api_key/privileged_features/*```, JWT is verified for role ```EchoSrv.API.Privileged"```
Both above routes execute call to remote service. Remote service url is defined using global variable.
2) protects calls to ```/api/api_key/``` using API Key and run actions set specific for the consumer
* First, the routing group verifies that current path is allowed for API Key that sent by client in header ```X-SUBSCRIPTION-KEY```
* Second, it runs actions defined for the specific consumer. Id of consumer is determined on previous step 
* Two routing groups ```/api/api_key/basic_features/*``` and ```/api/api_key/privileged_features/*``` specify unique and different ```Id``` that are used to allow appropriate URL path in API Key (subscription)
Both protected routes execute call to remote service. Remote service url is defined using global variable.
3) Url ```/ui/*``` is not protected, and used to serve static content from the preconfigured storage.


Echo service [configuration](../../Testing/CommonContent/Configuration/routing-config-echo.json). Simple service that responds inline string in HTTP body after 300ms delay.

Few [configurations](../../SecuredApi/Apps/Gateway.ComponentTests/TestEnvironment/Configuration/) used for component testing