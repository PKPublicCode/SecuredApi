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
using SecuredApi.WebApps.Gateway.Fixtures;

namespace SecuredApi.WebApps.Gateway;

[Collection("Gateway runner")]
public class EntraAuthGatewayTests : TestsBase
{
    private readonly EntraAuthenticationFixture _entra;

    public EntraAuthGatewayTests(GatewayHostFixture gateways, EntraAuthenticationFixture entra)
        : base(gateways)
    {
        _entra = entra;
    }

    [Fact]
    public async Task PositiveTest()
    {
        var token = await _entra.GetTokenAsync(default);
    }
}

