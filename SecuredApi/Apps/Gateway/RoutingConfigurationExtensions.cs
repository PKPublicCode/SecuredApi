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
using SecuredApi.Logic.Routing.Engine;
using SecuredApi.Logic.Routing.Engine.PartialRoutingTable;
using SecuredApi.Logic.Routing.Variables;
using SecuredApi.Logic.Routing.Json;
using SecuredApi.Logic.Routing.Utils;
using System.Net.Http;
using SecuredApi.Logic.Routing.Actions.Basic;
using Microsoft.AspNetCore.StaticFiles;
using SecuredApi.Apps.Gateway.Actions;
using SecuredApi.Apps.Gateway.Routing;
using SecuredApi.Apps.Gateway.Configuration;

namespace SecuredApi.Apps.Gateway
{
    public static class RoutingConfigurationExtensions
    {
        public static IServiceCollection ConfigureRoutingServices<FileAccessConfigurator>(this IServiceCollection srv, IConfiguration config)
            where FileAccessConfigurator: IInfrastructureConfigurator, new()
        {
            return srv.ConfigureRouter<FileAccessConfigurator>(config)
                .ConfigureVariables()
                .ConfigureStaticFilesAction<FileAccessConfigurator>(config);
        }

        public static IServiceCollection ConfigureStaticFilesAction<FileAccessConfigurator>(this IServiceCollection srv, IConfiguration config)
            where FileAccessConfigurator : IInfrastructureConfigurator, new()
        {
            return srv.ConfigureOptionalFeature(config, "StaticFilesProvider", (srv, config) =>
                        srv.ConfigureInfrastructure<ReturnStaticFileAction, FileAccessConfigurator>(config)
                            .AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>()
            );
        }

        public static IServiceCollection ConfigureRoutingHttpClients(this IServiceCollection srv)
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

        public static IServiceCollection ConfigureRouter<FileAccessConfigurator>(this IServiceCollection srv, IConfiguration config)
            where FileAccessConfigurator : IInfrastructureConfigurator, new()
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
    }
}
