# Routing configuration

Routing configuration is defined in json format with enabled comments.

# Main concepts
There are three key elements in the routing configuration
* Routes Group
* Route
* Action

The overall config structure can be depicted as following: 
```json
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
Routes Group allows you to group your routes with commonalities, like common beginning of the url path, that have common auth rules, required to run same actions, etc. JSON object has following structure:

```json
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

```json
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

```JSON
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

```json
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


https://github.com/PKPublicCode/SecuredApi/blob/d2c115f709bf75cc6fc025ae56d7403c5baf1ba1/Testing/CommonContent/Configuration/routing-config-gateway.json#L7

## Actions
Actions are executed one by one in order described in each section (see below). Execution order of sections corresponds to the parentheses: 

preRequestActions executed as: parent -> nested level 1 -> nested level 2, etc; 

onRequestErrorActions and onRequestSuccessActions executed in opposite order as: deepest route -> one level up -> two levels up -> ... -> parent

# Variables
Variable names are case insensitive

## Global variables
Format ```$(variableName)``` - available globally during config parsing stage. If variable name is unknown - parsing is stopped and configuration considered as invalid

## Runtime variables
in some occasions runtime variables (variables that has scope of the request) can be used.
Format ```@(variableName)```. Example is ```@(requestHttpMethod)```