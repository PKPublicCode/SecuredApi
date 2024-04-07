# Actions
## Summary
|Type|Gruard|Description|
|----|------|-----------|
|[DelayActionSettings](../../SecuredApi/Logic/Routing.Actions.Model/Basic/DelayActionSettings.cs)|No|Pauses processing of request for specified time interval |
|[CheckIPsActionSettings](../../SecuredApi/Logic/Routing.Actions.Model/Basic/CheckIPsActionSettings.cs)|Yes|Verifies inbound IP address |
## Details
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
