[CmdletBinding()]
param (
    [string]$gwAppName
)

# AzureAD module doesn't work in ps core, so... using az cli

$gwApiUri = "api://$gwAppName"
$clientAppName = "$gwAppName-client"

$roles = @{
    Read = "EchoSrv.Read.All"
    Write = "EchoSrv.Write.All" # will not be granted
}

$gwRolesJson = @(
    @{
        allowedMemberTypes = @(
            "Application"
        )
        description= "Allow get requests"
        displayName= "Reader"
        isEnabled= "true"
        value= $roles.Read
    },
    @{
        allowedMemberTypes= @(
            "Application"
        )
        description= "Allow post requests"
        displayName= "Writer"
        isEnabled= "true"
        value= $roles.Write
    }
) | ConvertTo-Json -Depth 100 -Compress

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
    if ($role.value -eq $roles.Read) {
        $readerRoleId = $role.id
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

#Delay for 10 secs to make sure all SPIs properly populated
Start-Sleep -s 10

#Grant permissions
az ad app permission add --id $clientApp.appId --api $gwApp.appId --api-permissions "$readerRoleId=Role" --only-show-errors
az ad app permission admin-consent --id $clientApp.appId

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