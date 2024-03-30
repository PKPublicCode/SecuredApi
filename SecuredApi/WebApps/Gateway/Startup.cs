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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using SecuredApi.Apps.Gateway;
using SecuredApi.Apps.Gateway.Azure.Configuration;

namespace SecuredApi.WebApps.Gateway;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration config)
    {
        _configuration = config;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Code-less instrumentation is not available for .net5 for linux. Will be available for .net6.
        // Don't forget remove Microsoft.ApplicationInsights.Profiler.AspNetCore package.
        services.AddApplicationInsightsTelemetry()
            .ConfigureRoutingServices<AzureFileAccessConfigurator>(_configuration)
            .ConfigureRoutingHttpClients()
            .ConfigureEntraAuthentication();

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRoutingMiddleware();
    }
}
