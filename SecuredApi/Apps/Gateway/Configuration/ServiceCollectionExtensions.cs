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

namespace SecuredApi.Apps.Gateway.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddScoped<TInterface1, TInterface2, TImplementation>(this IServiceCollection srv)
        where TImplementation : class, TInterface1, TInterface2
        where TInterface1 : class
        where TInterface2 : class
    {
        return srv.AddScoped<TImplementation>()
            .AddScoped<TInterface1>(s => s.GetRequiredService<TImplementation>())
            .AddScoped<TInterface2>(s => s.GetRequiredService<TImplementation>());
    }

    public static IServiceCollection AddSingleton<TInterface1, TInterface2, TImplementation>(this IServiceCollection srv)
        where TImplementation : class, TInterface1, TInterface2
        where TInterface1 : class
        where TInterface2 : class
    {
        return srv.AddSingleton<TImplementation>()
            .AddSingleton<TInterface1>(s => s.GetRequiredService<TImplementation>())
            .AddSingleton<TInterface2>(s => s.GetRequiredService<TImplementation>());
    }

    public static IServiceCollection ConfigureOptionalFeature(this IServiceCollection srv, IConfiguration config,
                                                                string name, Action<IServiceCollection, IConfigurationSection> configurator)
    {
        var featureCfg = config.GetSection(name);
        if (featureCfg != null)
        {
            configurator(srv, featureCfg);
        }
        return srv;
    }

    public static IServiceCollection ConfigureRequiredFeature(this IServiceCollection srv, IConfiguration config,
                                                                string name, Action<IServiceCollection, IConfigurationSection> configurator)
    {
        var featureCfg = config.GetSection(name)
            ?? throw new ConfigurationException($"Required feature {name} is not configured");
        configurator(srv, featureCfg);
        return srv;
    }
}
