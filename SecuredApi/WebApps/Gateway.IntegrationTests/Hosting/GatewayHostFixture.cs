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
namespace SecuredApi.WebApps.Gateway.Hosting;

public sealed class GatewayHostFixture: IDisposable
{
    private readonly GatewayRunner _gateway = new("appsettings-gateway.json");
    private readonly GatewayRunner _echo = new("appsettings-echo.json");
    private bool _started = false;

    public GatewayHostFixture()
    {
    }

    public HttpClient HttpClient => _gateway.Client;

    public async Task StartAsync()
    {
        if (!_started)
        {
            await _gateway.WarmupAsync();
            await _echo.WarmupAsync();
            _started = true;
        }
    }

    public void Dispose()
    {
        //XUnit doesn't call AsyncDisposable in some reason...
        _gateway.DisposeAsync().GetAwaiter().GetResult();
        _echo.DisposeAsync().GetAwaiter().GetResult();
    }
}

