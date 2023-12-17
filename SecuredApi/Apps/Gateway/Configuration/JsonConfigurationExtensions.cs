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
using SecuredApi.Logic.Routing.Json;
using SecuredApi.Logic.Routing;
using SecuredApi.Apps.Gateway.Routing;

namespace SecuredApi.Apps.Gateway.Configuration;

public static class JsonConfigurationExtensions
{
    public static IServiceCollection ConfigureOnTheFlyJsonParser(this IServiceCollection srv)
    {
        return srv.AddSingleton<IOnTheFlyRequestProcessor, OnTheFlyRequestProcessor>()
            .AddSingleton(new OnTheFlyRequestProcessorConfig
            (
                DefaultSerializerOptions: CommonSerializerOptions.Instance
            ));
    }

    public static IServiceCollection ConfigureRoutingConfigurationJsonParser(this IServiceCollection srv)
    {
        return srv.AddScoped<IRoutesParser, RoutesParser>()
            .AddSingleton<IRoutesParserConfig, RoutesParserConfig>()
            .AddSingleton<ICommonParserConfig, CommonParserConfig>();
    }

}
