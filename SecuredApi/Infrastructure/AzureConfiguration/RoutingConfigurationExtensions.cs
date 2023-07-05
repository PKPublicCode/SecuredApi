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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SecuredApi.Infrastructure.Configuration;
using SecuredApi.Logic.Routing.Actions.Subscriptions;
using SecuredApi.Infrastructure.Subscriptions.TableStorage;
using SecuredApi.Logic.Subscriptions;

namespace SecuredApi.Infrastructure.AzureConfiguration;

public static class RoutingConfigurationExtensions
{
    public static IServiceCollection ConfigureAzureSubscriptionManagement(this IServiceCollection srv, IConfiguration config)
    {
        return srv.ConfigureConsumers(config)
                .ConfigureSubscriptionKeys(config)
                .ConfigureSubscriptions(config);
    }

    public static IServiceCollection ConfigureConsumers(this IServiceCollection srv, IConfiguration config)
    {
        return srv.ConfigureOptionalFeature(config, "Consumers", (srv, config) =>
                    srv.ConfigureTableClientRepository<IConsumersRepository, ConsumersRepository>(config)
                        .AddSingleton<RunConsumerActionsAction>()
                        .ConfigureOnTheFlyJsonParser()
                );
    }

    public static IServiceCollection ConfigureSubscriptionKeys(this IServiceCollection srv, IConfiguration config)
    {
        return srv.ConfigureOptionalFeature(config, "SubscriptionKeys", (srv, config) =>
            srv.ConfigureTableClientRepository<ISubscriptionKeysRepository, SubscriptionKeysRepository>(config)
        );
    }

    public static IServiceCollection ConfigureSubscriptions(this IServiceCollection srv, IConfiguration config)
    {
        return srv.ConfigureOptionalFeature(config, "Subscriptions", (srv, config) =>
            srv.ConfigureTableClientRepository<ISubscriptionsRepository, SubscriptionsRepository>(config)
        );
    }
}

