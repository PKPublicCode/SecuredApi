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
using Microsoft.Extensions.Configuration;
using SecuredApi.Infrastructure.Subscriptions.TableStorage;

namespace SecuredApi.Apps.Gateway.Infrastructure
{
    public static class TableClientConfigurator
    {
        public static IServiceCollection ConfigureTableClientRepository<TInterface, TImplementation>(this IServiceCollection srv, IConfigurationSection cfg)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            return srv.AddSingleton<TInterface, TImplementation>()
                    .Configure<TableClientConfig<TInterface>>(cfg.GetRequiredSection("Repository"));
        }
    }
}
