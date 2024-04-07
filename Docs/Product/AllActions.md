# Actions
## Summary
### Basic
|Type|Guard|Description|
|----|------|-----------|
|[DelayActionSettings](#DelayActionSettings)|No|Pauses processing of request for specified time interval |
|[CheckIPsActionSettings](#CheckIPsActionSettings)|Yes|Verifies inbound IP address |
### Subscriptions
|Type|Guard|Description|
|----|------|-----------|
|[CheckSubscriptionActionSettings](#CheckSubscriptionActionSettings)|No|Verify if subscription key is valid and allowed for this route |
## Basic
### [DelayActionSettings](../../SecuredApi/Logic/Routing.Actions.Model/Basic/DelayActionSettings.cs)
#### Summary
Pauses processing of request for specified time interval 
#### Remarks
During the request this action waits for a specified time. Can be used to mimic load of the service(s). 
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
### [CheckIPsActionSettings](../../SecuredApi/Logic/Routing.Actions.Model/Basic/CheckIPsActionSettings.cs)
#### Summary
Verifies inbound IP address 
#### Parameters
|Name|Optional|Default Value|Description|
|----|--------|-------------|-----------|
|WhiteList|No||Array of the allowed IPs |
|NoAccessStatusCode|Yes|403|Status code returned in case of failure |
|NoAccessResponseBody|Yes|Empty string|Response body returned in case of failure. |
#### Return
True if IP found in a specified white list. False otherwise
## Subscriptions
### [CheckSubscriptionActionSettings](../../SecuredApi/Logic/Routing.Actions.Model/Subscriptions/CheckSubscriptionActionSettings.cs)
#### Summary
Verify if subscription key is valid and allowed for this route 
#### Parameters
|Name|Optional|Default Value|Description|
|----|--------|-------------|-----------|
|SubscriptionKeyHeaderName|No||Header name that bears subscription key |
|SuppressHeader|Yes|true|Removes this header from the outgoing request |
|ErrorNotAuthorizedBody|Yes|empty string|Customized body if key not valid |
|ErrorAccessDeniedBody|Yes|empty string|Customized body if key is valid, but not allowed for this routes group |
