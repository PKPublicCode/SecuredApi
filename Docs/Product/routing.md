## WIP

Routing configuration is defined in json format with enabled comments.

# Main concepts
There are three key elements
* RoutesGroup
* Route
* Action

github.com/PKPublicCode/SecuredApi/blob/d2c115f709bf75cc6fc025ae56d7403c5baf1ba1/Testing/CommonContent/Configuration/routing-config-gateway.json#L7

## Actions
Actions are executed one by one in order described in each section (see below). Execution order of sections corresponds to the parentheses: 

preRequestActions executed as: parent -> nested level 1 -> nested level 2, etc; 

onRequestErrorActions and onRequestSuccessActions executed in opposite order as: deepest route -> one level up -> two levels up -> ... -> parent

# Routing configuration format
## Root element
Has same format as ```RoutesGroup``` element with few restrictions and limitations:
* It can't have ```Routes``` property set. Only ```RoutesGroups``` property can be used to configure nested routes
* It has property ```routeNotFoundActions``` to configure actions

Sections:
### id
Optional field. Used to:
* specify allowed routes or routesGroups in Subscriptions
* useful to trace back errors in configuration (exception and log record contain id path to error element)

### description
Optional field and ignored by the parser. Conventionally used to provide human readable description

### preRequestActions 
Actions executed before proceeding to nested route or route group. If any of conditional actions failed (returned false), execution is stopped and execution fallbacks to onRequestErrorActions

### onRequestErrorActions
Executed if one of the conditional actions interrupted execution (action returned false), e.g. IP is not whitelisted. If conditional action returns false during processing of onRequestErrorActions actions, then behavior is the same and execution is interrupted.

### onRequestSuccessActions
Executed if all actions proceeded successfully (actions returned true)


TBD


RouteKey contains method and path and has to be unique, ignoring wildcard. That means that below routing keys considered to be equal and will trigger error
```
"routeKey": {
    "method": "get",
    "path": "/mypath/*"
}
```
```
"routeKey": {
    "method": "get",
    "path": "/mypath/"
}
```

# Variables
Variable names are case insensitive

## Global variables
Format ```$(variableName)``` - available globally during config parsing stage. If variable name is unknown - parsing is stopped and configuration considered as invalid

## Runtime variables
Not fully implemented and under consideration. However, in some ocasions runtime variables (variables that has scope of the request) can be used.
Format ```@(variableName)```. Example is ```@(requestHttpMethod)```