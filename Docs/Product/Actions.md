## WIP

### There will be list of all actions

#### ReturnStaticContent
Important: 
* if AutoDiscoverMimeType is not set to false (default is true), content type is determined on the fly by [IContentTypeProvider](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.staticfiles.icontenttypeprovider?view=aspnetcore-7.0) and set as appropriate header
* If route is setup to use wildcard, configured to return static file content, but file is not found, then Action will return 404 (with body if configured), but globalNotFound handler will not be executed. So, you'll receive just 404, without any other decorations that were setup for not-found routes.


[Actions registrations](Apps/Gateway/Actions/ActionFactoryBuilderExtensions.cs)