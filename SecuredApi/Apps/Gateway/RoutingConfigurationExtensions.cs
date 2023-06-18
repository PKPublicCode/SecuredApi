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
using SecuredApi.Logic.Routing;
using SecuredApi.Logic.Subscriptions;
using SecuredApi.Infrastructure.Subscriptions.TableStorage;
using SecuredApi.Logic.Routing.Engine;
using SecuredApi.Logic.Routing.Engine.PartialRoutingTable;
using SecuredApi.Logic.Routing.Variables;
using SecuredApi.Logic.Routing.Json;
using SecuredApi.Logic.Routing.Utils;
using System.Net.Http;
using SecuredApi.Logic.Routing.Actions.Basic;
using SecuredApi.Logic.Routing.Actions.Subscriptions;
using Microsoft.AspNetCore.StaticFiles;
using SecuredApi.Apps.Gateway.Infrastructure;
using SecuredApi.Apps.Gateway.Actions;
using SecuredApi.Apps.Gateway.Routing;

namespace SecuredApi.Apps.Gateway
{
    public static class RoutingConfigurationExtensions
    {
        public static IServiceCollection ConfigureRoutingServices(this IServiceCollection srv, IConfiguration config)
        {
            return srv.ConfigureHttpClients()
                .ConfigureRouter(config)
                .ConfigureVariables()
                .ConfigureStaticFilesAction(config)
                .ConfigureConsumers(config)
                .ConfigureSubscriptionKeys(config)
                .ConfigureSubscriptions(config);
        }

        public static IServiceCollection ConfigureStaticFilesAction(this IServiceCollection srv, IConfiguration config)
        {
            return srv.ConfigureOptionalFeature(config, "StaticFilesProvider", (srv, config) =>
                        srv.ConfigureInfrastructure<IStaticFileProvider, FileAccessConfigurator>(config)
                            .AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>()
            );
        }

        public static IServiceCollection ConfigureHttpClients(this IServiceCollection srv)
        {
            srv.AddHttpClient(); //Default http client
            srv.AddHttpClient(HttpClientNames.RemoteCallRedirectEnabled);
            srv.AddHttpClient(HttpClientNames.RemoteCallRedirectDisabled)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler() { AllowAutoRedirect = false } );
            return srv;
        }

        public static IServiceCollection ConfigureVariables(this IServiceCollection srv)
        {
            return srv.AddSingleton<IGlobalVariables, IGlobalVariablesUpdater, GlobalVariables>()
                .AddSingleton<IExpressionProcessor, ExpressionProcessor>()
                .AddSingleton<IGlobalVariablesStreamParser, GlobalVariablesJsonParser>()
                .AddTransient<IDefaultGlobalVariablesProvider>(srvs =>
                            new DefaultGlobalVariablesProvider(
                                    srvs.GetRequiredService<IConfiguration>().GetSection("GlobalVariables")
                                ));
        }

        public static IServiceCollection ConfigureRouter(this IServiceCollection srv, IConfiguration config)
        {
            return srv.AddSingleton<IRouter, IRouterUpdater, Router>()
                    
                    .ConfigureRequiredFeature(config, "RoutingEngineManager", (srv, config) =>
                        srv.ConfigureInfrastructure<IRoutingEngineManager, FileAccessConfigurator>(config)
                            .AddTransient<IRoutingEngineManager, RoutingEngineManager>()
                            .Configure<RoutingEngineManagerCfg>(config.GetRequiredSection("Files"))
                            .AddSingleton<IRoutingTableBuilderFactory, RoutingTableBuilderFactory<RoutingTableBuilder>>()
                            .ConfigureRoutingConfigurationJsonParser()
                            .AddActionFactory()
                        );
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
}
