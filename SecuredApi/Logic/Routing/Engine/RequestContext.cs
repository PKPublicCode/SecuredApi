// Copyright (c) 2021 - present, Pavlo Kruglov.
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the Server Side Public License, version 1,
// as published by MongoDB, Inc.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// Server Side Public License for more details.
//
// You should have received a copy of the Server Side Public License
// along with this program. If not, see
// <http://www.mongodb.com/licensing/server-side-public-license>.
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SecuredApi.Logic.Variables;

namespace SecuredApi.Logic.Routing.Engine;

public class RequestContext: IRequestContext, IDisposable
{
    private Response _response;

    public HttpContext HttpContext { get; }
    public HttpRequest Request => HttpContext.Request;
    public IResponse Response => _response;
    public RouteRecord Route { get; }
    public IRuntimeVariables Variables { get; }
    public CancellationToken CancellationToken => HttpContext.RequestAborted;
    public IServiceProvider ServiceProvider => HttpContext.RequestServices;
    public ConnectionInfo ConnectionInfo => HttpContext.Connection;

    public RequestContext(RouteRecord route, HttpContext httpContext)
    {
        Route = route;
        HttpContext = httpContext;
        Variables = httpContext.RequestServices.GetRequiredService<IRuntimeVariables>();
        _response = new Response(HttpContext.Response);
    }

    public void Dispose()
    {
        _response.Dispose();
    }
}
