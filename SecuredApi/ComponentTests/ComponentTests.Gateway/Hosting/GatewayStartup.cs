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
using RichardSzalay.MockHttp;
using SecuredApi.Apps.Gateway;
using SecuredApi.Apps.Gateway.Configuration;
using SecuredApi.Logic.Routing.Utils;

namespace SecuredApi.ComponentTests.Gateway.Hosting;

public class GatewayStartup
{
    private readonly IConfiguration _configuration;

    protected MockHttpMessageHandler CommonHttpHandler { get; } = new();
    protected MockHttpMessageHandler MainHttpHandler { get; } = new();
    protected MockHttpMessageHandler NonRedirectHttpHandler { get; } = new();

    public GatewayStartup(IConfiguration config)
    {
        _configuration = config;
    }

    public void ConfigureServices(IServiceCollection srv)
    {
        srv.ConfigureRoutingServices<FileAccessConfigurator>(_configuration);

        srv.AddHttpClient(string.Empty)
            .ConfigurePrimaryHttpMessageHandler(() => CommonHttpHandler); //Default http client

        srv.AddHttpClient(HttpClientNames.RemoteCallRedirectEnabled)
            .ConfigurePrimaryHttpMessageHandler(() => MainHttpHandler);

        srv.AddHttpClient(HttpClientNames.RemoteCallRedirectDisabled)
            .ConfigurePrimaryHttpMessageHandler(() => NonRedirectHttpHandler);

        MainHttpHandler.Fallback.Throw(new InvalidOperationException("No matching mock handler"));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRoutingMiddleware();
    }
}

