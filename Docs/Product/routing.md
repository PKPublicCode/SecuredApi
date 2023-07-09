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