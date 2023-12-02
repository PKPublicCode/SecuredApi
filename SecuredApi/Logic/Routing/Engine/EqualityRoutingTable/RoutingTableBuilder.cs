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

namespace SecuredApi.Logic.Routing.Engine.EqualityRoutingTable;

public class RoutingTableBuilder: RoutingTableBuilderBase
{
    private readonly Dictionary<RouteKey, RouteRecord> _routes = new(new RouteKeyComparer());

    public override void AddRoute(string path, string method, RouteRecord route)
    {
        _routes.Add(new RouteKey(method, path).ToLower(), route);
    }

    public override IRoutingTable Build() => new RoutingTable(_routes, NotFoundRoute);

    private class RouteKeyComparer : IEqualityComparer<RouteKey>
    {
        public bool Equals(RouteKey x, RouteKey y)
        {
            return (string.Compare(x.Method, y.Method) == 0)
                && (string.Compare(x.Path, y.Path) == 0);
        }

        public int GetHashCode(RouteKey obj)
        {
            return HashCode.Combine(obj.Path, obj.Method);
        }
    }
}
