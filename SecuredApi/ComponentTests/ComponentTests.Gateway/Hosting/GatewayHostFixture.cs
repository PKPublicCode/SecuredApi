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
using SecuredApi.Logic.Routing.Utils;
using System.Net;

namespace SecuredApi.ComponentTests.Gateway.Hosting;

public class GatewayHostFixture: IDisposable
{
    private CancellationTokenSource _stopToken = new();

    protected MockHttpMessageHandler CommonHttpHandler { get; } = new();
    protected MockHttpMessageHandler MainHttpHandler { get; } = new();
    protected MockHttpMessageHandler NonRedirectHttpHandler { get; } = new();

    public HttpClient Client = new();

    public GatewayHostFixture()
    {
    }

    public async Task StartAsync(string config)
    {
        MainHttpHandler.Fallback.Throw(new InvalidOperationException("No matching mock handler"));

        int port = 5001;
        var task = InternalHost.RunHost<GatewayStartup>(
            config,
            port,
            srv =>
                {
                    srv.AddHttpClient(string.Empty)
            .           ConfigurePrimaryHttpMessageHandler(() => CommonHttpHandler); //Default http client
                    srv.AddHttpClient(HttpClientNames.RemoteCallRedirectEnabled)
                        .ConfigurePrimaryHttpMessageHandler(() => MainHttpHandler);
                    srv.AddHttpClient(HttpClientNames.RemoteCallRedirectDisabled)
                        .ConfigurePrimaryHttpMessageHandler(() => NonRedirectHttpHandler);
                },
            _stopToken.Token);

        Client.BaseAddress = new Uri($"http://localhost:{port}/");

        await Warmup();
    }

    private async Task Warmup()
    {
        do
        {
            var v = await Client.GetAsync("");
            //Define codes to validate
            if (v.StatusCode == HttpStatusCode.NotFound)
            {
                break;
            }
        }
        while(true);
    }

    public void Dispose()
    {
        _stopToken.Cancel();
        Client.Dispose();
    }
}

