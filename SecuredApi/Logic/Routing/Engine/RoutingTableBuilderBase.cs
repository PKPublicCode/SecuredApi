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

namespace SecuredApi.Logic.Routing.Engine
{
    public abstract class RoutingTableBuilderBase : IRoutingTableBuilder
    {
        private RouteRecord? _notFoundRecord;
        protected RouteRecord NotFoundRoute => _notFoundRecord ?? throw new RouteConfigurationException("Not Found Route not configured");

        public void AddNotFoundRoute(RouteRecord route)
        {
            _notFoundRecord = route;
        }

        public abstract void AddRoute(string path, string method, RouteRecord route);


        public abstract IRoutingTable Build();
    }
}
