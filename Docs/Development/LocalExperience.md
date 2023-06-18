WIP

### Local testing
### Start azure storage emulator
azurite --oauth basic --cert cert.pem --key key.pem
### Simple echo
dotnet SecuredApi.WebApps.Gateway.Echo.dll --urls https://localhost:9001;http://localhost:9000