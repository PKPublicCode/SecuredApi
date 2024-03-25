[CmdletBinding()]
param (
    [string]$gwAppName
)

if (!$gwAppName) {
    "Specify application display name"
    exit
}

# AzureAD module doesn't work in ps core, so... using az cli

$gwApiUri = "api://$gwAppName"
$clientAppName = "$gwAppName-client"

$gwRoles = @(
    @{
        allowedMemberTypes = @(
            "Application"
        )
        description= "Grants access to basic features"
        displayName= "Basic access"
        isEnabled= "true"
        value= "EchoSrv.API.Basic"
    },
    @{
        allowedMemberTypes= @(
            "Application"
        )
        description= "Grants access to restricted API"
        displayName= "Privileged access"
        isEnabled= "true"
        value= "EchoSrv.API.Privileged"
    }
)

$gwRolesJson = $gwRoles | ConvertTo-Json -Depth 100 -Compress
$basicRole = $gwRoles[0].value
$privilegedRole = $gwRoles[1].value

$currentUserId = (az ad signed-in-user show | ConvertFrom-Json).id

#create gateway app
$gwApp = az ad app create `
    --display-name $gwAppName `
    --identifier-uris $gwApiUri `
    --only-show-errors `
    | ConvertFrom-Json

az ad app owner add --id $gwApp.appId --owner-object-id $currentUserId
az ad app update --id $gwApp.appId --app-roles $gwRolesJson
az ad sp create --id $gwApp.appId

#find role id
$appRoles = (az ad app show --id $gwApp.appId | ConvertFrom-Json).appRoles
foreach($role in $appRoles) {
    if ($role.value -eq $basicRole) {
        $basicRoleId = $role.id
        break
    }
}

#create client app
$clientApp = az ad app create `
    --display-name $clientAppName `
    --only-show-errors `
    | ConvertFrom-Json

az ad app owner add --id $clientApp.appId --owner-object-id $currentUserId

$clientAppCredentials = az ad app credential reset --id $clientApp.appId | ConvertFrom-Json

#Grant permissions

#Wait for 10 secs to make sure all SPIs properly populated.
Start-Sleep -s 10
az ad app permission add --id $clientApp.appId --api $gwApp.appId --api-permissions "$basicRoleId=Role" --only-show-errors

#Wait for 10 secs to to make above changes populated
Start-Sleep -s 30
az ad app permission admin-consent --id $clientApp.appId

"If pemission still not grantd run:"
"az ad app permission admin-consent --id $($clientApp.appId)"

#$tenantId = (az account show | ConvertFrom-Json).tenantId
#Show results
"
Copy-paste this to .bash_profile :
export SECAPI_IT_MAIN__SpiClient__TenantId=`"$($clientAppCredentials.tenant)`"
export SECAPI_IT_MAIN__SpiClient__ClientId=`"$($clientAppCredentials.appId)`"
export SECAPI_IT_MAIN__SpiClient__ClientSecret=`"$($clientAppCredentials.password)`"
export SECAPI_IT_MAIN__SpiClient__Scope=`"$($gwApiUri)/.default`"

export SECAPI_IT_GW__Globals__Variables__AllowedEntraTokenIssuer=`"https://sts.windows.net/$($clientAppCredentials.tenant)/`"
export SECAPI_IT_GW__Globals__Variables__AllowedEntraTokenAudience=`"$($gwApiUri)`"
"