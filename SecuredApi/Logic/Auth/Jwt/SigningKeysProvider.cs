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

namespace SecuredApi.Logic.Auth.Jwt;

public class SigningKeysProvider: ISigningKeysProvider
{
    public SigningKeysProvider()
    {
    }

    public async Task<IEnumerable<SecurityKey>> GetKeysAsync(string issuer, CancellationToken ct)
    {
        string stsDiscoveryEndpoint = "https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration";

        ConfigurationManager<OpenIdConnectConfiguration> configManager
            = new ConfigurationManager<OpenIdConnectConfiguration>(stsDiscoveryEndpoint, new OpenIdConnectConfigurationRetriever());

        OpenIdConnectConfiguration config = await configManager.GetConfigurationAsync();

        return config.JsonWebKeySet.Keys;
    }
}

