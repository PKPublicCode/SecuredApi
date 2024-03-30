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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using SecuredApi.Logic.Routing;
using SecuredApi.Logic.Routing.Engine;
using SecuredApi.Logic.Routing.Engine.PartialRoutingTable;
using SecuredApi.Logic.Variables;
using SecuredApi.Logic.Routing.Json;
using SecuredApi.Logic.Routing.Utils;
using SecuredApi.Logic.Routing.Actions.Basic;
using Microsoft.AspNetCore.StaticFiles;
using SecuredApi.Apps.Gateway.Actions;
using SecuredApi.Apps.Gateway.Routing;
using SecuredApi.Apps.Gateway.Configuration;
using SecuredApi.Logic.Auth.Subscriptions;
using SecuredApi.Logic.Routing.Actions.Subscriptions;

namespace SecuredApi.Apps.Gateway;

public static class RoutingConfigurationExtensions
{
    public static IServiceCollection ConfigureRoutingServices<FileAccessConfigurator>(this IServiceCollection srv, IConfiguration config)
        where FileAccessConfigurator : IInfrastructureConfigurator, new()
    {
        return srv.ConfigureRouter<FileAccessConfigurator>(config)
            .ConfigureVariables()
            .ConfigureStaticFilesAction<FileAccessConfigurator>(config)
            .ConfigureSubscriptions<FileAccessConfigurator>(config);
    }

    public static IServiceCollection ConfigureRoutingHttpClients(this IServiceCollection srv)
    {
        srv.AddHttpClient(); //Default http client
        srv.AddHttpClient(HttpClientNames.RemoteCallRedirectEnabled);
        srv.AddHttpClient(HttpClientNames.RemoteCallRedirectDisabled)
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler() { AllowAutoRedirect = false });
        return srv;
    }

    private static IServiceCollection ConfigureStaticFilesAction<FileAccessConfigurator>(this IServiceCollection srv, IConfiguration config)
        where FileAccessConfigurator : IInfrastructureConfigurator, new()
    {
        return srv.ConfigureOptionalFeature(config, "StaticFilesProvider", (srv, config) =>
                    srv.ConfigureInfrastructure<ReturnStaticFileAction, FileAccessConfigurator>(config)
                        .AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>()
        );
    }

    private static IServiceCollection ConfigureVariables(this IServiceCollection srv)
    {
        return srv.AddSingleton<IGlobalVariables, IGlobalVariablesUpdater, GlobalVariables>()
            .AddSingleton<IGlobalExpressionProcessor, GlobalExpressionProcessor>()
            .AddSingleton<IRuntimeExpressionParser, RuntimeExpressionParser>()
            .AddSingleton<IGlobalVariablesStreamParser, GlobalVariablesJsonParser>()
            .AddScoped<IRuntimeVariables, RuntimeVariables>()
            .AddTransient<IDefaultGlobalVariablesProvider>(srvs =>
                        new DefaultGlobalVariablesProvider(
                                srvs.GetRequiredService<IConfiguration>().GetSection("Globals:Variables")
                            ));
    }

    private static IServiceCollection ConfigureRouter<FileAccessConfigurator>(this IServiceCollection srv, IConfiguration config)
        where FileAccessConfigurator : IInfrastructureConfigurator, new()
    {
        return srv.AddSingleton<IRouter, IRouterUpdater, Router>()

                .ConfigureRequiredFeature(config, "RoutingEngineManager", (srv, config) =>
                    srv.ConfigureInfrastructure<IRoutingEngineManager, FileAccessConfigurator>(config)
                        .AddTransient<IRoutingEngineManager, RoutingEngineManager>()
                        .ConfigureRequiredOption<RoutingEngineManagerCfg>(config, "Files")
                        .AddSingleton<IRoutingTableBuilderFactory, RoutingTableBuilderFactory<RoutingTableBuilder>>()
                        .ConfigureRoutingConfigurationJsonParser()
                        .AddActionFactory()
                    );
    }

    private static IServiceCollection ConfigureSubscriptions<FileAccessConfigurator>(this IServiceCollection srv, IConfiguration rootConfig)
        where FileAccessConfigurator : IInfrastructureConfigurator, new()
    {
        return srv
            .ConfigureOptionalFeature(rootConfig, "Subscriptions:Keys", (srv, config) =>
                    srv.ConfigureInfrastructure<ISubscriptionKeysRepository, FileAccessConfigurator>(config)
                        .AddSingleton<ISubscriptionKeysRepository, SubscriptionKeysRepository>()
                        .AddSingleton<IHashCalculator, Sha256HashCalculator>()
                        .ConfigureRequiredOption<SubscriptionsSecurityCfg>(config, "Security")
                )
            .ConfigureOptionalFeature(rootConfig, "Subscriptions:Consumers", (srv, config) =>
                    srv.ConfigureInfrastructure<IConsumersRepository, FileAccessConfigurator>(config)
                        .AddSingleton<IConsumersRepository, ConsumersRepository>()
                        .ConfigureOnTheFlyJsonParser()
                        .AddSingleton<RunConsumerActionsAction>()
                )
               ;
    }
}
