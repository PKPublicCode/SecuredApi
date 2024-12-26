# Runtime Functions
## Common
|Name|Description|
|----|-----------|
|getRemainingPath|Returns remained path that was captured by * (asterisk) for the rote. For example, if route was defined for ```/api/some_feature/*```, and received request with path ```/api/some_feature/a/b/c/d``` then remained path will be ```a/b/c/d``` |
|getRequestMethod|Returns Http Method of client request |
|getQueryString|Returns query string of the client request. For example, if route was defined for ```/api/some_feature```, and receives request with path ```/api/some_feature?a=1&b=2``` then query string will be ```a=1&b=2`` |
|getQueryParam|Returns parameter of query string. If parameter is not set, returns empty string. For example, if route was defined for ```/api/some_feature```, and receives request with path ```/api/some_feature?someParam=1&anotherParam=2``` then query ```getQueryParam(someParam)``` will return ```1``` |
|getVariable|Returns runtime variable. For example ```getVariable(requestRemainingPath)``` will return the same string as function ```getRemainingPath()``` |
|transformQueryString|Rebuilds query string into the new string using new parameter names, beginning, equality and split characters (strings).<br><br>Signature:<br>``` rebuildQueryString('beginningString', 'splitString', 'equalityString', 'oldParameterName:newParameterName', ...)```<br>Number of parameters is limited by 256. Parameters that doesn't have specified name mapping will be ignored and omitted from output string. Order of the parameters in the result string satisfies the alphanumerical order of the new parameter names. If query has no parameters with specified mapping, then resulted string will be empty string<br><br>Example:<br>Request's query string: ```?param3=30&param2=20&param1=10```<br>Function: ```transformQueryString('_b_', '_s_', '_e_', 'param1:newParam1', 'param2:newParam2'```)<br>Result: ```_b_newParam1_e_10_s_newParam2_20```<br>|
