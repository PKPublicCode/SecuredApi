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

## Variables
Variables allow to parametrize routing configuration. There are two types of variables: Global and Runtime. All variable names are case sensitive and has to be alpha-numeric string started with letter.

### Global variables
Global variables are immutable parameters defined outside of the routing configuration by the gateway owner loaded before parsing and initialization of routing configuration. Main purpose is to use same routing configuration file for different deployment environments.

Format: ```$(variableName)```. If variable is not found during routing configuration parsing and initialization, error is thrown and whole configuration is ignored.

Global variables can be used as part of any string value of the routing config file and substituted by the appropriate value during configuration initialization process, that makes zero performance impact during the executing request.

How to configure global variables see [here](./GlobalVariablesConfiguration.md)

### Runtime variables
Runtime variables are set by the routing engine and actions during request execution. In contrast to global variables, runtime variables scope is request, and values depend on specific request, actions that were executed and their results. Only action properties with type ```RuntimeExpression``` allow using runtime variables. In this case variables substituted by values just before action execution.

Format: ```@(variableName)```. Runtime variables are not known and can't be validated during the parsing and initialization of routing configuration.  If variable is not set for this request, error will be thrown during the execution.

In below example, incoming request will be passed to the service defined by application owner as ```protectedEchoPath``` and can be different for different environments. Method of outgoing request is ```httpRequestMethod``` that means the same as request method. ```requestRemainingPath``` is a url path that corresponds to the asterisk and is remaining part after removing ```/resource_a/``` in the beginning.

[List of available variables](./RuntimeVariables.md)

```json5
"Routes": [ 
  {
    "description": "Requests for Resource A",
    "path": "/resource_a/*",
    "methods": [ "get", "post" ],
    "Actions": [
      {
        "type": "RemoteCall",
        "path": "$(remoteServerPath)/@(requestRemainingPath)", 
        "method": "@(requestHttpMethod)"
      }
    ]
  }
]
```

So, if global variable defined as ```"remoteServerPath": "http://myserver/api"```json5, and client sends request ```[get] /resource_a/resource_x/resource_y```, then outgoing request according to above configuration will be ```[get] http://myserver/api/resource_x/resource_y```

# Examples
Gateway [configuration](../../Testing/CommonContent/Configuration/routing-config-gateway.json) used for integration tests. Configuration defines protection of downstream api with Entra Jwt Token validation (/api/jwt/ path), with api key (api/api_key path), and serves static files (/ui/ path).

Echo service [configuration](../../Testing/CommonContent/Configuration/routing-config-echo.json). Simple service that responds inline string in HTTP body after 300ms delay.

Few [configurations](../../SecuredApi/Apps/Gateway.ComponentTests/TestEnvironment/Configuration/) used for component testing