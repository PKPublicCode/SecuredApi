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
using SecuredApi.Apps.Gateway.Configuration;
using SecuredApi.Apps.Gateway;
using SecuredApi.Logic.Routing;
using Microsoft.AspNetCore.Http;
using RichardSzalay.MockHttp;
using SecuredApi.Logic.Routing.Utils;

namespace SecuredApi.ComponentTests.Gateway;

public abstract class GatewayTestsBase
{
    private const string _defaultFileName = "appsettings-common.json";

    protected readonly IServiceProvider _serviceProvider;
    protected readonly HttpContext Context = new DefaultHttpContext();
    protected HttpRequest Request => Context.Request;
    protected HttpResponse Response => Context.Response;
    protected string ResponseBody => ReadStream(Response.Body);
    protected MockHttpMessageHandler CommonHttpHandler { get; } = new ();
    protected MockHttpMessageHandler MainHttpHandler { get; } = new ();
    protected MockHttpMessageHandler NonRedirectHttpHandler { get; } = new ();
    protected ExpectedResult ExpectedResult { get; } = new();

    protected GatewayTestsBase()
        :this(_defaultFileName, (x, y) => { })
    {
    }

    protected GatewayTestsBase(string fileName, Action<IServiceCollection, IConfiguration> configurator)
    {
        IConfiguration cfg = new ConfigurationBuilder()
                                .AddJsonFile(fileName)
                                .Build();

        var srv = new ServiceCollection()
                   .AddScoped(_ => cfg)
                   .ConfigureRoutingServices<FileAccessConfigurator>(cfg);

        srv.AddHttpClient(string.Empty)
            .ConfigurePrimaryHttpMessageHandler(() => CommonHttpHandler); //Default http client

        srv.AddHttpClient(HttpClientNames.RemoteCallRedirectEnabled)
            .ConfigurePrimaryHttpMessageHandler(() => MainHttpHandler);

        srv.AddHttpClient(HttpClientNames.RemoteCallRedirectDisabled)
            .ConfigurePrimaryHttpMessageHandler(() => NonRedirectHttpHandler);
        
        MainHttpHandler.Fallback.Throw(new InvalidOperationException("No matching mock handler"));

        configurator(srv, cfg);

        _serviceProvider = srv.BuildServiceProvider();

        Context.RequestServices = _serviceProvider;
        Context.Response.Body = new MemoryStream();
    }

    protected virtual async Task ArrangeAsync()
    {
        using var scope = _serviceProvider.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<IRoutingEngineManager>()
                        .InitializeRoutingEngineAsync(Context.RequestAborted);
    }

    protected virtual async Task ActAsync()
    {
        using var scope = _serviceProvider.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<IRouter>()
                                    .ProcessAsync(Context);
    }

    protected virtual Task AssertAsync()
    {
        Response.StatusCode.Should().Be(ExpectedResult.StatusCode);
        Response.Headers.Should().BeEquivalentTo(ExpectedResult.Headers);
        ResponseBody.Should().Be(ExpectedResult.Body);
        return Task.CompletedTask;
    }

    protected virtual async Task ExecuteAsync()
    {
        // Intentionally run in different ServiceProvider scopes
        await ArrangeAsync();
        await ActAsync();
        await AssertAsync();
    }

    private static string ReadStream(Stream s)
    {
        s.Position = 0;
        using var reader = new StreamReader(s);
        return reader.ReadToEnd();
    }
}

