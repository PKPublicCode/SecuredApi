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

namespace SecuredApi.Apps.Gateway.Engine
{
    public class RoutingInitializer : IHostedService
    {
        private IServiceScopeFactory _serviceScopeFactory;

        //IHostedService is singleton, so unable to inject transient services dirrectly
        //That's why scope service factory is injected. Then services registered with any scope can be used
        public RoutingInitializer(IServiceScopeFactory serviceScopedFactory)
        {
            _serviceScopeFactory = serviceScopedFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IRoutingEngineManager>();
            await manager.InitializeRoutingEngineAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
