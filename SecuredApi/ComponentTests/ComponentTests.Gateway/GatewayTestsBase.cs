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
using SecuredApi.Apps.Gateway;
using SecuredApi.Logic.Routing;
using Microsoft.AspNetCore.Http;
using FluentAssertions;

namespace SecuredApi.ComponentTests.Gateway;

public class GatewayTestsBase
{
    private const string _defaultFileName = "appsettings.json";
    private readonly IServiceProvider _serviceProvider;

    protected readonly HttpContext Context = new DefaultHttpContext();

    protected HttpRequest Request => Context.Request;
    public HttpResponse Response => Context.Response;

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
        configurator(srv, cfg);

        _serviceProvider = srv.BuildServiceProvider();
    }

    protected async Task ArrangeAsync(CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<IRoutingEngineManager>()
                        .InitializeRoutingEngineAsync(ct);
    }

    protected async Task ActAsync()
    {
        using var scope = _serviceProvider.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<IRouter>()
                                    .ProcessAsync(Context);
    }

    protected async Task ExecuteAsync()
    {
        // Intentionally run in different ServiceProvider Scopes
        await ArrangeAsync(Context.RequestAborted);
        await ActAsync();
    }
}

