## WIP

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

## Actions
Actions are executed one by one in order described in each section (see below). Execution order of sections corresponds to the parentness: 

preRequestActions executed as: parent -> nested level 1 -> nested level 2, etc; 

onRequestErrorActions and onRequestSuccessActions executed in opposite order as: deepest route -> one level up -> two levels up -> ... -> parent

Sections:
### preRequestActions 
Actions executed before proceeding to nested route or route group. If any of conditional actions failed (returned false), execution is stopped and execution fallbacks to onRequestErrorActions

### onRequestErrorActions
Executed if one of the conditional actions interrupted execution (action returned false), e.g. IP is not whitelisted. If conditional action returns false during processing of onRequestErrorActions actions, then behaviour is the same and execution is interrupted.

### onRequestSuccessActions
Executed if all actions proceeded succesfully (actions returnned true)