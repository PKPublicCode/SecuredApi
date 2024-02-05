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
using System.Net;

namespace SecuredApi.Apps.Gateway.Azure.Hosting;

public class GatewayRunner: IAsyncDisposable
{
    private CancellationTokenSource _stopToken = new();
    private readonly Task? _task;
    public HttpClient Client { get; } = new();
    public string HostAddress { get; }

    public GatewayRunner(string config)
    {
        HostAddress = GetHostUrl(config);
        Client.BaseAddress = new Uri(HostAddress);

        _task = InternalHost.RunHostAsync<GatewayStartup>(
            config,
            srv => { },
            _stopToken.Token);
    }

    public async Task WarmupAsync()
    {
        do
        {
            var v = await Client.GetAsync("");
            //Define codes to validate
            if (v.StatusCode == HttpStatusCode.NotFound)
            {
                break;
            }
            await Task.Delay(100);
        }
        while (true);
    }

    public async ValueTask DisposeAsync()
    {
        _stopToken.Cancel();
        Client.Dispose();

        if (_task != null)
        {
            await _task;
        }
    }

    private string GetHostUrl(string configFileName)
    {
        return new ConfigurationBuilder()
                .AddJsonFile(configFileName)
                .Build()
                .GetRequiredSection("Kestrel:EndPoints:Http:Url").Get<string>()
                    ?? throw new InvalidOperationException("Kestrel config not set");
    }
}

