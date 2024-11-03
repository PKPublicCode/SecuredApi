# Runtime Functions
## Common
|Name|Description|
|----|-----------|
|getRemainingPath|Returns remained path that was captured by * (asterisk) for the rote. For example, if route was defined for ```/api/some_feature/*```, and received request with path ```/api/some_feature/a/b/c/d``` then remained path will be ```a/b/c/d``` |
|getRequestMethod|Returns Http Method of client request |
|getQueryString|Returns query string of the client request. For example, if route was defined for ```/api/some_feature```, and receives request with path ```/api/some_feature?a=1&b=2``` then query string will be ```a=1&b=2`` |
|getQueryParam|Returns parameter of query string. If parameter is not set, returns empty string. For example, if route was defined for ```/api/some_feature```, and receives request with path ```/api/some_feature?someParam=1&anotherParam=2``` then query ```getQueryParam(someParam)``` will return ```1``` |
|getVariable|Returns runtime variable. For example ```getVariable(requestRemainingPath)``` will return the same string as function ```getRemainingPath()``` |
