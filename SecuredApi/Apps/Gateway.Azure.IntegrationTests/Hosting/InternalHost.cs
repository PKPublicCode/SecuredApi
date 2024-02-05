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
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SecuredApi.Apps.Gateway;

namespace SecuredApi.Apps.Gateway.Azure.Hosting;

public static class InternalHost
{
    public static Task RunHostAsync<TStartup>(string config, Action<IServiceCollection> configurator, CancellationToken ct)
        where TStartup: class
    {
        return Host.CreateDefaultBuilder()
            .ConfigureRoutingInitializer()
            .ConfigureAppConfiguration(cfgBuilder =>
            {
                cfgBuilder.AddJsonFile(config, false);
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<TStartup>()
                    .ConfigureServices(configurator);
                    //.UseUrls($"http://localhost:{port}")
            })
            .Build()
            .RunAsync(ct);
    }
}

