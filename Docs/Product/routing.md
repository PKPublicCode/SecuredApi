## WIP

# Main concept
Three key elements
* RoutesGroup
* Route
* Action

## Actions
Actions are executed one by one in order described in each section (see below). Execution order of sections corresponds to the parentness: 

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
* usefull to trace back errors in configuration (exception and log record contain id path to error element)

### description
Optional field and ignored by the parser. Convenionally used to provide human readable description

### preRequestActions 
Actions executed before proceeding to nested route or route group. If any of conditional actions failed (returned false), execution is stopped and execution fallbacks to onRequestErrorActions

### onRequestErrorActions
Executed if one of the conditional actions interrupted execution (action returned false), e.g. IP is not whitelisted. If conditional action returns false during processing of onRequestErrorActions actions, then behaviour is the same and execution is interrupted.

### onRequestSuccessActions
Executed if all actions proceeded succesfully (actions returnned true)


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