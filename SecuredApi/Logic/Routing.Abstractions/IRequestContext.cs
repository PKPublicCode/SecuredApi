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

namespace SecuredApi.Logic.Routing;

public interface IRequestContext
{
    public HttpRequest Request { get; }
    public IResponse Response { get; }
    public RouteRecord Route { get; }
    public IServiceProvider ServiceProvider { get; }
    public string RemainingPath { get; }
    public IDictionary<string, object> Variables { get; }
    public CancellationToken CancellationToken { get; }
    public ConnectionInfo ConnectionInfo { get; }
}
