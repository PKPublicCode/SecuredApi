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
using System.Net;
using SecuredApi.Testing.Common;
using SecuredApi.Apps.Gateway.Configuration;
using static SecuredApi.WebApps.Gateway.Utils.Constants.RoutePaths;
using static SecuredApi.Testing.Common.Jwt.SigningKeys;
using static SecuredApi.Testing.Common.Jwt.TokenHelper;

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
    public async Task ProtectedByReadRoleRoute_TokenWithReadRole_Success()
    {
        Request.SetPost()
            .SetStringContent("Hello hello")
            .SetRelativePath(PrivateOAuthRedirectWildcard);
        AddAuthorizationHeader(await _entra.GetTokenAsync(default));

        ExpectedResult.Body = InlineContent.EchoDelay;
        ExpectedResult.StatusCode = HttpStatusCode.OK;
        ExpectedResult.AddHeaders(Headers.ResponseEchoServerRequestProcessed);

        await ActAsync();
        await AssertAsync();
    }

    [Fact]
    public async Task ProtectedByWriteRoleRoute_TokenWithReadRole_NotAuthorized()
    {
        Request.SetPost()
            .SetStringContent("Hello hello")
            .SetRelativePath(PrivateOAuthNotAllowedWildcard);
        AddAuthorizationHeader(await _entra.GetTokenAsync(default));

        ExpectedResult.StatusCode = HttpStatusCode.Forbidden;
        ExpectedResult.AddHeaders(Headers.ResponseCommonOnError);
        ExpectedResult.Body = InlineContent.AccessDenied;

        await ActAsync();
        await AssertAsync();
    }

    [Fact]
    public async Task ProtectedByWriteRoleRoute_SignedByWrongKeys_NotAuthorized()
    {
        Request.SetPost()
            .SetStringContent("Hello hello")
            .SetRelativePath(PrivateApiKeyRedirectWildcard);

        var config = Configuration.Build("appsettings-tests");
        var token = CreateJwtToken(config.GetRequiredSection("Globals:Variables:AllowedEntraTokenIssuer").GetRequired<string>(),
                                    config.GetRequiredSection("Globals:Variables:AllowedEntraTokenAudience").GetRequired<string>(),
                                    TestKey2,
                                    new [] {"EchoSrv.Read.All", "EchoSrv.Write.All"},
                                    DateTime.UtcNow,
                                    TimeSpan.FromHours(1));
        AddAuthorizationHeader(token);

        ExpectedResult.StatusCode = HttpStatusCode.Unauthorized;
        ExpectedResult.AddHeaders(Headers.ResponseCommonOnError);
        ExpectedResult.Body = InlineContent.SubscriptionKeyNotSetOrInvalid;

        await ActAsync();
        await AssertAsync();
    }

    private void AddAuthorizationHeader(string token)
    {
        Request.AddHeader(Headers.AuthorizationHeaderName, OAuthHeaderPrefix + token);
    }
}

