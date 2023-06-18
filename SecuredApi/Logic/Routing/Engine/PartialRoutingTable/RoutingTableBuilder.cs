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
using System.Linq;

namespace SecuredApi.Logic.Routing.Engine.PartialRoutingTable
{
    public class RoutingTableBuilder : RoutingTableBuilderBase
    {
        private readonly Dictionary<string, RoutesTree> _routeTrees = new();

        public override void AddRoute(string path, string method, RouteRecord route)
        {
            method = method.ToLower();
            if (!_routeTrees.TryGetValue(method, out var tree))
            {
                tree = new();
                _routeTrees[method] = tree;
            }
            tree.AddRoute(path, route);
        }

        public override IRoutingTable Build()
        {
            return new RoutingTable(_routeTrees.ToDictionary(x => x.Key, x => (IRoutesTree)x.Value), NotFoundRoute);
        }
    }
}
