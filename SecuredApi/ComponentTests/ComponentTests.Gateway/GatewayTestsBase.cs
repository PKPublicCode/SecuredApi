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

namespace Tests.Component.Gateway;

public class GatewayTestsBase
{
    private const string _defaultFileName = "appsettings.json";
    private readonly IServiceProvider _serviceProvider;

    protected readonly HttpContext HttpContext = new DefaultHttpContext();

    protected HttpRequest Request => HttpContext.Request;
    public HttpResponse Response => HttpContext.Response;

    public GatewayTestsBase()
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

    protected async Task ArrangeAsync()
    {
        using var scope = _serviceProvider.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<IRoutingEngineManager>()
                        .InitializeRoutingEngineAsync(HttpContext.RequestAborted);
    }

    protected async Task ActAsync()
    {
        using var scope = _serviceProvider.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<IRouter>()
                                    .ProcessAsync(HttpContext);
    }

    [Fact]
    public async Task Test1()
    {
        await ArrangeAsync();
        Request.Path = "/echo/success";
        Request.Method = HttpMethods.Get;

        await ActAsync();

        Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }
}

