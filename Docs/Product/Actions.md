# Actions
## Summary
### Basic
|Type|Fallible|Description|
|----|------|-----------|
|[Delay](#Delay)|No|Pauses processing of request for specified time interval. |
|[ReturnStaticFile](#ReturnStaticFile)|Yes|Returns static content (file) to the client. |
|[RemoteCall](#RemoteCall)|Yes|Makes outgoing http(s) call to downstream service and send current state of the client request. Response of the service, including headers, status code and body is saved to the client response |
|[CheckIPs](#CheckIPs)|Yes|Verifies inbound IP address |
### Subscriptions
|Type|Fallible|Description|
|----|------|-----------|
|[CheckSubscription](#CheckSubscription)|No|Verify if subscription key is valid and allowed for this route |
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
Makes outgoing http(s) call to downstream service and send current state of the client request. Response of the service, including headers, status code and body is saved to the client response 
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
## Subscriptions
### [CheckSubscription](../../SecuredApi/Logic/Routing.Actions.Model/Subscriptions/CheckSubscription.cs)
#### Summary
Verify if subscription key is valid and allowed for this route 
#### Parameters
|Name|Optional|Default Value|Description|
|----|--------|-------------|-----------|
|SubscriptionKeyHeaderName|No||Header name that bears subscription key |
|SuppressHeader|Yes|true|Removes this header from the outgoing request |
|ErrorNotAuthorizedBody|Yes|empty string|Customized body if key not valid |
|ErrorAccessDeniedBody|Yes|empty string|Customized body if key is valid, but not allowed for this routes group |
