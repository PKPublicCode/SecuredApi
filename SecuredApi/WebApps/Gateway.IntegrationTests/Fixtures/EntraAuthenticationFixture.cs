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
using Azure.Identity;
using Azure.Core;
using SecuredApi.Testing.Common;

namespace SecuredApi.WebApps.Gateway.Fixtures;

public class EntraAuthenticationFixture
{
    private readonly TokenCredential _tokenCredential;
    private readonly SpiClient _clientConfig;
    private string? _token; 

    public EntraAuthenticationFixture()
    {
        var config = Configuration.Build("appsettings-tests", "SECURED_API_INTEGRATION_TESTS__");

         _clientConfig = config.GetRequiredSection("SpiClient").Get<SpiClient>()
                        ?? throw new InvalidOperationException("Entra client not configured");
        _tokenCredential = new ClientSecretCredential(_clientConfig.TenantId, _clientConfig.ClientId, _clientConfig.ClientSecret);
    }

    public async Task<string> GetTokenAsync(CancellationToken ct)
    {
        if (_token == null)
        {
            var ctx = new TokenRequestContext(new[] { _clientConfig.Scope });
            var result = await _tokenCredential.GetTokenAsync(ctx, ct);
            _token = result.Token;
        }

        return _token;
    }


    private class SpiClient
    {
        public string TenantId { get; init; } = string.Empty;
        public string ClientId { get; init; } = string.Empty;
        public string ClientSecret { get; init; } = string.Empty;
        public string Scope { get; init; } = string.Empty;
    }
}


