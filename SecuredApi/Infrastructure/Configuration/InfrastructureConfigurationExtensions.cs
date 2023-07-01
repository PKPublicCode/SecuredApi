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

namespace SecuredApi.Infrastructure.Configuration;
public static class InfrastructureConfigurationExtensions
{
    public static IServiceCollection ConfigureInfrastructure<TClient, TConfigurator>(this IServiceCollection srv, IConfiguration config, string cfgPath, bool required = true)
        where TConfigurator : IInfrastructureConfigurator, new()
    {
        var cfgSection = config.GetSection(cfgPath);

        if (cfgSection == null)
        {
            if (required)
            {
                throw new ConfigurationException($"Required infrastructure {cfgPath} is not configured");
            }
            return srv;
        }

        return srv.ConfigureInfrastructure<TClient, TConfigurator>(cfgSection, required);
    }

    public static IServiceCollection ConfigureInfrastructure<TClient, TConfigurator>(this IServiceCollection srv, IConfigurationSection config, bool required = true)
       where TConfigurator : IInfrastructureConfigurator, new()
    {
        var configurator = new TConfigurator();
        string sectionName = configurator.SectionName;
        var section = config.GetSection(sectionName);

        if (section == null)
        {
            if (required)
            {
                throw new ConfigurationException($"Required infrastructure {config.Path}:{sectionName} is not configured");
            }
            return srv;
        }

        string name = section.GetValue<string>("type")
                        ?? throw new ConfigurationException($"Type of infrastructure {config.Path} is not set");
        var configuratorAction = configurator.GetConfigurator<TClient>(name)
                        ?? throw new ConfigurationException($"Type {name} for infrustructure {config.Path} is unknown");
        configuratorAction(srv, section);
        return srv;
    }
}
