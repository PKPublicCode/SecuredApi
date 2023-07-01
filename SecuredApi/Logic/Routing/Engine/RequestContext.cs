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
using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Http;

namespace SecuredApi.Logic.Routing.Engine
{
    public class RequestContext: IRequestContext, IDisposable
    {
        private Response _response;

        public HttpContext HttpContext { get; }
        public HttpRequest Request => HttpContext.Request;
        public IResponse Response => _response;
        public IReadOnlyList<RoutesGroup> RoutesGroups { get; }
        public string RemainingPath { get; }
        public IDictionary<string, object> Variables { get; } = new Dictionary<string, object>();
        public CancellationToken CancellationToken => HttpContext.RequestAborted;
        public IServiceProvider ServiceProvider => HttpContext.RequestServices;
        public ConnectionInfo ConnectionInfo => HttpContext.Connection;

        public RequestContext(IReadOnlyList<RoutesGroup> routesGroups, 
                                        string remainingPath, 
                                        HttpContext httpContext)
        {
            RoutesGroups = routesGroups;
            RemainingPath = remainingPath;
            HttpContext = httpContext;
            _response = new Response(HttpContext.Response);
        }

        public void Dispose()
        {
            _response.Dispose();
        }
    }
}