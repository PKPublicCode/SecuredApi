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
namespace SecuredApi.Logic.Routing.Engine.PartialRoutingTable;

internal class RoutingTable : IRoutingTable
{
    private Dictionary<string, IRoutesTree> _trees;
    private RouteInfo _notFoundRoute;

    public RoutingTable(Dictionary<string, IRoutesTree> routes, RouteRecord notFoundRoute)
    {
        _trees = routes;
        _notFoundRoute = new RouteInfo(notFoundRoute, string.Empty);
    }

    public Task<RouteInfo> GetRoutingAsync(string path, string method, CancellationToken token)
    {
        method = method.ToLower();
        if (_trees.TryGetValue(method, out var tree))
        {
            if (tree.TryFindRote(path, out var info))
            {
                return Task.FromResult(info);
            }
        }
        return Task.FromResult(_notFoundRoute);
    }
}
