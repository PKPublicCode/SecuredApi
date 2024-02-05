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
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Collections.Concurrent;

namespace SecuredApi.Logic.Auth.Jwt;

public class SigningKeysProvider: ISigningKeysProvider
{
    private readonly ConcurrentDictionary<string, ConfigurationManager<OpenIdConnectConfiguration>> _configs = new(StringComparer.OrdinalIgnoreCase);
    private readonly SemaphoreSlim _lock = new(1);
    private const string _wellknownSuffix = "/v2.0/.well-known/openid-configuration";

    public SigningKeysProvider()
    {
    }

    public async Task<IEnumerable<SecurityKey>> GetKeysAsync(string issuer, CancellationToken ct)
    {
        if (!_configs.TryGetValue(issuer, out var configManager))
        {
            configManager = await SafeAddConfigurationAsync(issuer, ct);
        }

        //According to current implementation, ConfigurationManager is threadsafe and caches configuration
        var config = await configManager.GetConfigurationAsync();

        return config.JsonWebKeySet.Keys;
    }

    private async Task<ConfigurationManager<OpenIdConnectConfiguration>> SafeAddConfigurationAsync(string issuer, CancellationToken ct)
    {
        await _lock.WaitAsync(ct);
        try
        {
            // Check again, if value was added during awating the lock
            if (_configs.TryGetValue(issuer, out var config))
            {
                return config;
            }

            string stsDiscoveryEndpoint = issuer + _wellknownSuffix;

            //ToDo find way to inject configurable HttpClient
            config = new ConfigurationManager<OpenIdConnectConfiguration>(
                            stsDiscoveryEndpoint,
                            new OpenIdConnectConfigurationRetriever()
                         );
            _configs.TryAdd(issuer, config);
            return config;
        }
        finally
        {
            _lock.Release();
        }
    }
}

