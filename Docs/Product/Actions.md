# Actions
## Summary
### Auth
|Type|Fallible|Description|
|----|------|-----------|
|[RunConsumerActions](#RunConsumerActions)|No|Runs actions configured for the specified consumer. |
|[CheckEntraJwtClaims](#CheckEntraJwtClaims)|No|Checks claims of the entra jwt.  |
|[CheckEntraJwt](#CheckEntraJwt)|Yes|Verifies that JWT is signed by proper keys, has valid issuer and issued for valid audience. |
|[CheckSubscription](#CheckSubscription)|Yes|Verify the subscription key (api key) and checks if subscription is allowed for this route |
### Basic
|Type|Fallible|Description|
|----|------|-----------|
|[Delay](#Delay)|No|Pauses processing of request for specified time interval. |
|[SetResponseHeader](#SetResponseHeader)|No|Adds new header to the client request. If header already exists, the another key-value pair will be added |
|[SuppressResponseHeaders](#SuppressResponseHeaders)|No|Removes header from client response |
|[SetResponse](#SetResponse)|No|Sets client response |
|[ReturnStaticFile](#ReturnStaticFile)|Yes|Returns static content (file) to the client. |
|[RemoteCall](#RemoteCall)|Yes|Makes outgoing http(s) call to remote service using current state of the client request. Response of the service, including headers, status code and body is saved to the client response |
|[SetRequestInfoToResponse](#SetRequestInfoToResponse)|No|Writes client request information to the body of client response. Main usage is debugging and troubleshooting. |
|[CheckIPs](#CheckIPs)|Yes|Verifies inbound IP address |
|[SuppressRequestHeaders](#SuppressRequestHeaders)|No|Removes header from client request |
|[SetRequestHeader](#SetRequestHeader)|No|Adds new header to the client response. If header already exists, the another key-value pair will be added |
## Auth
### [RunConsumerActions](../../SecuredApi/Logic/Routing.Actions.Model/Auth/RunConsumerActions.cs)
#### Summary
Runs actions configured for the specified consumer. 
#### Remarks
Action has no parameters. Action just takes Consumer Id preserved by the CheckSubscription action, loads actions configured for the consumer, and executes them 
#### Parameters
No parameters
### [CheckEntraJwtClaims](../../SecuredApi/Logic/Routing.Actions.Model/Auth/CheckEntraJwtClaims.cs)
#### Summary
Checks claims of the entra jwt.  
#### Remarks
This action should go only after CheckEntraJwt action. In some cases it's more convenient to CheckEntaJwt for group of routes, but check different claims for different routes in this group. 
#### Parameters
|Name|Optional|Default Value|Description|
|----|--------|-------------|-----------|
|OneOfRoles|Yes|null|Sets one of roles that must be set in the JWT |
|OneOfScopes|Yes|null|Sets one of scopes that must be set the JWT |
### [CheckEntraJwt](../../SecuredApi/Logic/Routing.Actions.Model/Auth/CheckEntraJwt.cs)
#### Summary
Verifies that JWT is signed by proper keys, has valid issuer and issued for valid audience. 
#### Remarks
Validation is designed for and tested with Entra (Azure AD) json web tokens. Other auth servers're comming soon 
#### Parameters
|Name|Optional|Default Value|Description|
|----|--------|-------------|-----------|
|OneOfIssuers|No||One of allowed issuers |
|OneOfAudiences|No||One of expected audiences |
|OneOfRoles|Yes|null |One of roles that must be in the JWT |
|OneOfScopes|Yes|null |One of scopes that must be in the JWT |
|HeaderName|Yes|"Authorization" |HTTP Header name that bears JWT token |
|TokenPrefix|Yes|"Bearer " |Prefix in the header value, that bears JWT token |
|KeepData|Yes|false |Whether parsed JWT token object shold remain in the memory and used by further actions, or can be released. If CheckEntraJwtClaims action is used later for this route, then value shold be true. |
#### Return
Fails in following cases:<br>I. Not authorized; Set 401 status code to client response:<br>* JWT token malformed<br>* JWT token doesn't signed by key that correspond to the provided issuer<br>II. Access Denied; Sets 403 status code to client response:<br>* Token issuer is invalid<br>* Audience is invalid<br>* Roles are invalid<br>* Scopes are invalid 
### [CheckSubscription](../../SecuredApi/Logic/Routing.Actions.Model/Auth/CheckSubscription.cs)
#### Summary
Verify the subscription key (api key) and checks if subscription is allowed for this route 
#### Parameters
|Name|Optional|Default Value|Description|
|----|--------|-------------|-----------|
|SubscriptionKeyHeaderName|No||Header name that bears subscription key |
|SuppressHeader|Yes|true|Removes this header from the outgoing request |
|ErrorNotAuthorizedBody|Yes|empty string|Customized body if key not valid |
|ErrorAccessDeniedBody|Yes|empty string|Customized body if key is valid, but not allowed for this routes group |
#### Return
Action fails if:<br>* Subscription key header doesn't exist, or empty. In this case it sets response code 401 (Not Authorized)<br>* Subscription key (api key) is invalid (or doesn't exists). In this case it sets response code 401 (Not Authorized)<br>* Subscription key (api key) is valid, but route is not allowed to run. In this case response code set to 401 (Access denied) 
## Basic
### [Delay](../../SecuredApi/Logic/Routing.Actions.Model/Basic/Delay.cs)
#### Summary
Pauses processing of request for specified time interval. 
#### Remarks
During the request this action waits for a specified time. No interaction with the client request or response happens. Can be used to mimic load of the service(s). 
#### Parameters
|Name|Optional|Default Value|Description|
|----|--------|-------------|-----------|
|Milliseconds|No||Time to wait |
#### Example
```jsonc

            {
                "type":"delay"
                "Milliseconds": 300
            }
            
```
### [SetResponseHeader](../../SecuredApi/Logic/Routing.Actions.Model/Basic/SetResponseHeader.cs)
#### Summary
Adds new header to the client request. If header already exists, the another key-value pair will be added 
#### Parameters
|Name|Optional|Default Value|Description|
|----|--------|-------------|-----------|
|Name|No||Header name |
|Value|No||Value of the header |
### [SuppressResponseHeaders](../../SecuredApi/Logic/Routing.Actions.Model/Basic/SuppressResponseHeaders.cs)
#### Summary
Removes header from client response 
#### Parameters
|Name|Optional|Default Value|Description|
|----|--------|-------------|-----------|
|Headers|No||List of header names to be removed from the response |
### [SetResponse](../../SecuredApi/Logic/Routing.Actions.Model/Basic/SetResponse.cs)
#### Summary
Sets client response 
#### Parameters
|Name|Optional|Default Value|Description|
|----|--------|-------------|-----------|
|HttpCode|No||Http code that is set to client response |
|Body|No||Body |
### [ReturnStaticFile](../../SecuredApi/Logic/Routing.Actions.Model/Basic/ReturnStaticFile.cs)
#### Summary
Returns static content (file) to the client. 
#### Remarks
Files can be stored either on the file system or in the storage account. See StaticFileProvider configuration 
#### Parameters
|Name|Optional|Default Value|Description|
|----|--------|-------------|-----------|
|Path|No||Relative path to the file. |
|NotFoundMessage|Yes|empty string|String that is written to the client response body if file wasn't found |
|AutoDiscoverMimeType|Yes|true|If set to true, tries automatically discover mime type depending on the file name and adds appropriate header to client response. For more details read about [IContentTypeProvider](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.staticfiles.icontenttypeprovider?view=aspnetcore-7.0) |
#### Return
Fails if file not found. Set HTTP Code 404 to client response in this case 
### [RemoteCall](../../SecuredApi/Logic/Routing.Actions.Model/Basic/RemoteCall.cs)
#### Summary
Makes outgoing http(s) call to remote service using current state of the client request. Response of the service, including headers, status code and body is saved to the client response 
#### Parameters
|Name|Optional|Default Value|Description|
|----|--------|-------------|-----------|
|Path|No||Url of the downstream service. Allows using runtime variables |
|Method|No||HTTP Method used to call downstream service |
|Timeout|Yes|-1 (infinite)|Timeout in milliseconds that used for outgoing http call. If timeout occurs, gateway chain set as failed and status code set to 504 (Gateway timeout) |
|EnableRedirect|Yes|true|If true and remote service replies redirect code, action automatically calls redirected location and write redirected call to the client response.<br>If false, HTTP Redirect code received from remote server is not validated and is written to client response as is. Client will be responsible to handle redirect response himself. |
#### Return
Fails only if timeout occured. Succeeds otherwise. 
#### Example
```jsonc

            {
              "type": "RemoteCall",
              "path": "https://www.google.com/@(requestRemainingPath)",
              "method": "get"
              "timeout": 500
            }
            
```
### [SetRequestInfoToResponse](../../SecuredApi/Logic/Routing.Actions.Model/Basic/SetRequestInfoToResponse.cs)
#### Summary
Writes client request information to the body of client response. Main usage is debugging and troubleshooting. 
#### Remarks
Action writes to the client response following:<br>* Host<br>* Request Path<br>* Request Path Base<br>* Method<br>* Headers<br>* Inbound IP<br>
#### Parameters
|Name|Optional|Default Value|Description|
|----|--------|-------------|-----------|
|HttpCode|Yes|200 |HTTP code set to the client response |
|HeadLine|Yes|"Debug information:" |Headline added before request information |
### [CheckIPs](../../SecuredApi/Logic/Routing.Actions.Model/Basic/CheckIPs.cs)
#### Summary
Verifies inbound IP address 
#### Remarks
Inbound ip address is taken from the client HTTP request properties. 
#### Parameters
|Name|Optional|Default Value|Description|
|----|--------|-------------|-----------|
|WhiteList|No||Array of the allowed IPs |
|NoAccessStatusCode|Yes|403|Status code returned in case of failure |
|NoAccessResponseBody|Yes|Empty string|Response body returned in case of failure. |
#### Return
Secceeded if IP found in a specified white list. Fails otherwise otherwise 
### [SuppressRequestHeaders](../../SecuredApi/Logic/Routing.Actions.Model/Basic/SuppressRequestHeaders.cs)
#### Summary
Removes header from client request 
#### Parameters
|Name|Optional|Default Value|Description|
|----|--------|-------------|-----------|
|Headers|No||List of header names to be removed from the request |
### [SetRequestHeader](../../SecuredApi/Logic/Routing.Actions.Model/Basic/SetRequestHeader.cs)
#### Summary
Adds new header to the client response. If header already exists, the another key-value pair will be added 
#### Parameters
|Name|Optional|Default Value|Description|
|----|--------|-------------|-----------|
|Name|No||Header name |
|Value|No||Value of the header |
