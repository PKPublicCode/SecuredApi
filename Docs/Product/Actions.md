# Actions
## Summary
### Basic
|Type|Fallible|Description|
|----|------|-----------|
|[SetRequestInfoToResponse](#SetRequestInfoToResponse)|No|Writes client request information to the body of client response. Main usage is debugging and troubleshooting. |
|[SetResponse](#SetResponse)|No|Sets client response |
|[SuppressRequestHeaders](#SuppressRequestHeaders)|No|Removes header from client request |
|[SetRequestHeader](#SetRequestHeader)|No|Adds new header to the client response. If header already exists, the another key-value pair will be added |
|[SuppressResponseHeaders](#SuppressResponseHeaders)|No|Removes header from client response |
|[RemoteCall](#RemoteCall)|Yes|Makes outgoing http(s) call to remote service using current state of the client request. Response of the service, including headers, status code and body is saved to the client response |
|[SetResponseHeader](#SetResponseHeader)|No|Adds new header to the client request. If header already exists, the another key-value pair will be added |
|[Delay](#Delay)|No|Pauses processing of request for specified time interval. |
|[ReturnStaticFile](#ReturnStaticFile)|Yes|Returns static content (file) to the client. |
|[CheckIPs](#CheckIPs)|Yes|Verifies inbound IP address |
### Auth
|Type|Fallible|Description|
|----|------|-----------|
|[RunConsumerActions](#RunConsumerActions)|Yes|Runs actions configured for the specified consumer. |
|[CheckEntraJwt](#CheckEntraJwt)|Yes|Verifies that JWT is signed by proper keys, has valid issuer and issued for valid audience. |
|[CheckSubscription](#CheckSubscription)|Yes|Verify the subscription key (api key) and checks if subscription is allowed for this route |
|[CheckEntraJwtClaims](#CheckEntraJwtClaims)|Yes|Checks claims of the Entra jwt.  |
## Basic
### [SetRequestInfoToResponse](../../SecuredApi/Logic/Routing.Model/ActionsActions.Basic/SetRequestInfoToResponse.cs)
#### Summary
Writes client request information to the body of client response. Main usage is debugging and troubleshooting. 
#### Remarks
Action writes to the client response following:<br>* Host<br>* Request Path<br>* Request Path Base<br>* Method<br>* Headers<br>* Inbound IP<br>
#### Parameters
|Name|Optional|Default Value|Description|
|----|--------|-------------|-----------|
|HttpCode|Yes|200 |HTTP code set to the client response |
|HeadLine|Yes|"Debug information:" |Headline added before request information |
### [SetResponse](../../SecuredApi/Logic/Routing.Model/ActionsActions.Basic/SetResponse.cs)
#### Summary
Sets client response 
#### Parameters
|Name|Optional|Default Value|Description|
|----|--------|-------------|-----------|
|HttpCode|No||Http code that is set to client response |
|Body|No||Body |
### [SuppressRequestHeaders](../../SecuredApi/Logic/Routing.Model/ActionsActions.Basic/SuppressRequestHeaders.cs)
#### Summary
Removes header from client request 
#### Parameters
|Name|Optional|Default Value|Description|
|----|--------|-------------|-----------|
|Headers|No||List of header names to be removed from the request |
### [SetRequestHeader](../../SecuredApi/Logic/Routing.Model/ActionsActions.Basic/SetRequestHeader.cs)
#### Summary
Adds new header to the client response. If header already exists, the another key-value pair will be added 
#### Parameters
|Name|Optional|Default Value|Description|
|----|--------|-------------|-----------|
|Name|No||Header name |
|Value|No||Value of the header |
### [SuppressResponseHeaders](../../SecuredApi/Logic/Routing.Model/ActionsActions.Basic/SuppressResponseHeaders.cs)
#### Summary
Removes header from client response 
#### Parameters
|Name|Optional|Default Value|Description|
|----|--------|-------------|-----------|
|Headers|No||List of header names to be removed from the response |
### [RemoteCall](../../SecuredApi/Logic/Routing.Model/ActionsActions.Basic/RemoteCall.cs)
#### Summary
Makes outgoing http(s) call to remote service using current state of the client request. Response of the service, including headers, status code and body is saved to the client response 
#### Parameters
|Name|Optional|Default Value|Description|
|----|--------|-------------|-----------|
|Path|No||Url of the downstream service. Allows using runtime variables |
|Method|No||HTTP Method used to call downstream service |
|Timeout|Yes|-1 (infinite)|Timeout in milliseconds that used for outgoing http call. If timeout occurs, gateway chain set as failed and status code set to 504 (Gateway timeout) |
|EnableRedirect|Yes|true|If true and remote service replies redirect code, action automatically calls redirected location and write redirected call to the client response.<br>If false, HTTP Redirect code received from remote server is not validated and is written to client response as is. Client will be responsible to handle redirect response himself. |
#### Fallibility
Fails only if timeout occurred. Succeeds otherwise. 
#### Example
```jsonc

            {
              "type": "RemoteCall",
              "path": "https://www.google.com/@(requestRemainingPath)",
              "method": "get",
              "timeout": 500
            }
            
```
### [SetResponseHeader](../../SecuredApi/Logic/Routing.Model/ActionsActions.Basic/SetResponseHeader.cs)
#### Summary
Adds new header to the client request. If header already exists, the another key-value pair will be added 
#### Parameters
|Name|Optional|Default Value|Description|
|----|--------|-------------|-----------|
|Name|No||Header name |
|Value|No||Value of the header |
### [Delay](../../SecuredApi/Logic/Routing.Model/ActionsActions.Basic/Delay.cs)
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
### [ReturnStaticFile](../../SecuredApi/Logic/Routing.Model/ActionsActions.Basic/ReturnStaticFile.cs)
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
#### Fallibility
Fails if file not found. Set HTTP Code 404 to client response in this case 
### [CheckIPs](../../SecuredApi/Logic/Routing.Model/ActionsActions.Basic/CheckIPs.cs)
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
#### Fallibility
Succeeds if IP is found in a specified white list. Fails otherwise otherwise 
## Auth
### [RunConsumerActions](../../SecuredApi/Logic/Routing.Model/ActionsActions.Auth/RunConsumerActions.cs)
#### Summary
Runs actions configured for the specified consumer. 
#### Remarks
Executes actions configured for the current consumer (client). RunConsumerActions has to be exectuted after one of authentication actions, that saves current consumer id. 
#### Parameters
|Name|Optional|Default Value|Description|
|----|--------|-------------|-----------|
|IgnoreIfAbsent|Yes|false|Configures behavior when record for customer is not found: If false and there are no record, then call will fail with 500 Http Error. If true and there are no record, error will be ignored and execution pass to the next route action. |
#### Fallibility
Fails when:<br>* one of consumer actions fails. HTTP code in client response is set according to the consumer action<br>* if consumer id is invalid, not found, or CheckSubscription action wasn't executed for this rote. In this case 500 HTTP code is set to client response, indicating that data is corrupted<br>If consumer actions are successful (if any), the action succeeds . 
### [CheckEntraJwt](../../SecuredApi/Logic/Routing.Model/ActionsActions.Auth/CheckEntraJwt.cs)
#### Summary
Verifies that JWT is signed by proper keys, has valid issuer and issued for valid audience. 
#### Remarks
Validation is designed for and tested with Entra (Azure AD) json web tokens. Other auth servers're coming. 
#### Parameters
|Name|Optional|Default Value|Description|
|----|--------|-------------|-----------|
|OneOfIssuers|No||One of allowed issuers |
|OneOfAudiences|No||One of expected audiences |
|OneOfRoles|Yes|null |One of roles that must be in the JWT |
|OneOfScopes|Yes|null |One of scopes that must be in the JWT |
|HeaderName|Yes|"Authorization" |HTTP Header name that bears JWT token |
|TokenPrefix|Yes|"Bearer " |Prefix in the header value, that bears JWT token |
|KeepData|Yes|false |Whether parsed JWT token object should remain in the memory and used by further actions, or can be released. If CheckEntraJwtClaims action is used later for this route, then value should be set to true. |
|ConsumerIdClaim|Yes|null |If not null, sets value of the specified claim to ConsumerId, that can be used later for ```RunConsumerActions``` action |
#### Fallibility
Fails in following cases:<br>I. Not authorized; Set 401 status code to client response:<br>* JWT token malformed<br>* JWT token doesn't signed by key that correspond to the provided issuer<br>II. Access Denied; Sets 403 status code to client response:<br>* Token issuer is invalid<br>* Audience is invalid<br>* Roles are invalid<br>* Scopes are invalid 
### [CheckSubscription](../../SecuredApi/Logic/Routing.Model/ActionsActions.Auth/CheckSubscription.cs)
#### Summary
Verify the subscription key (api key) and checks if subscription is allowed for this route 
#### Parameters
|Name|Optional|Default Value|Description|
|----|--------|-------------|-----------|
|SubscriptionKeyHeaderName|No||Header name that bears subscription key |
|SuppressHeader|Yes|true|Removes this header from the outgoing request |
|ErrorNotAuthorizedBody|Yes|empty string|Customized body if key not valid |
|ErrorAccessDeniedBody|Yes|empty string|Customized body if key is valid, but not allowed for this routes group |
#### Fallibility
Action fails if:<br>* Subscription key header doesn't exist, or empty. Response's HTTP status code is set to 401 (Not Authorized)<br>* Subscription key (api key) is invalid (or doesn't exists). Response's HTTP status code is set to 401 (Not Authorized)<br>* Subscription key (api key) is valid, but route is not allowed to run. Response's HTTP status code is set to 403 (Access denied) 
### [CheckEntraJwtClaims](../../SecuredApi/Logic/Routing.Model/ActionsActions.Auth/CheckEntraJwtClaims.cs)
#### Summary
Checks claims of the Entra jwt.  
#### Remarks
This action should go only after CheckEntraJwt action. In some cases it's more convenient to CheckEntraJwt for group of routes, but check different claims for different routes in this group. 
#### Parameters
|Name|Optional|Default Value|Description|
|----|--------|-------------|-----------|
|OneOfRoles|Yes|null|Sets one of roles that must be set in the JWT |
|OneOfScopes|Yes|null|Sets one of scopes that must be set the JWT |
#### Fallibility
Fails if JWT doesn't satisfy one of roles, or one of scopes specified in the parameters. In this case sets http code to 403 (access denied) in the client response. 
