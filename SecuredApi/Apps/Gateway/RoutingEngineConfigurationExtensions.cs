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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SecuredApi.Apps.Gateway.Engine;
using Microsoft.AspNetCore.Builder;

namespace SecuredApi.Apps.Gateway
{
    public static class RoutingEngineConfigurationExtensions
    {
        public static IServiceCollection AddRoutingInitializer(this IServiceCollection services)
        {
            return services.AddHostedService<RoutingInitializer>();
        }

        public static IHostBuilder ConfigureRoutingInitializer(this IHostBuilder builder)
        {
            return builder.ConfigureServices(services =>
            {
                services.AddHostedService<RoutingInitializer>();
            });
        }

        public static IApplicationBuilder UseRoutingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RoutingMiddleware>();
        }
    }
}
