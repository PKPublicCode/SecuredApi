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
using Microsoft.Extensions.Hosting;
using SecuredApi.Logic.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
namespace SecuredApi.Apps.Gateway.Engine;

public class RoutingConfigurationLoader : IHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private CancellationTokenSource? _stop;
    private Task? _backgroundJob;

    //IHostedService is singleton, so unable to inject transient services dirrectly
    //That's why scope service factory is injected. Then services registered with any scope can be used
    public RoutingConfigurationLoader(IServiceScopeFactory serviceScopedFactory)
    {
        _serviceScopeFactory = serviceScopedFactory;
    }

    public async Task StartAsync(CancellationToken ct)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        await ReloadAsync(scope, ct);

        _stop = CancellationTokenSource.CreateLinkedTokenSource(ct);
        _backgroundJob = StartBackgroundJob();
    }

    public async Task StopAsync(CancellationToken ct)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var config = scope.ServiceProvider.GetRequiredService<IOptions<RoutingConfigurationLoaderCfg>>();
        int timeout = config.Value.ShutdownTimeout;

        _stop?.Cancel();
        try
        {
            await (_backgroundJob?.WaitAsync(TimeSpan.FromMilliseconds(timeout), ct)
            ?? Task.CompletedTask);
        }
        catch(Exception e)
            when (e is TaskCanceledException || e is TimeoutException)
        {
            //Just ignore it
        }
    }

    private async Task StartBackgroundJob()
    {
        while (!_stop!.IsCancellationRequested)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var config = scope.ServiceProvider.GetRequiredService<IOptions<RoutingConfigurationLoaderCfg>>();
            await Task.Delay(config.Value.ReloadFrequency, _stop.Token);
            await ReloadAsync(scope, _stop.Token);
        }
    }

    private async Task ReloadAsync(IServiceScope scope, CancellationToken ct)
    {
        var manager = scope.ServiceProvider.GetRequiredService<IRoutingEngineManager>();
        await manager.InitializeRoutingEngineAsync(ct);
    }
}
