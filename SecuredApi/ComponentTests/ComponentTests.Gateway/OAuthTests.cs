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
using SecuredApi.Logic.Auth.Jwt;
using RichardSzalay.MockHttp;
using System.Net;
using static SecuredApi.Testing.Common.Jwt.SigningKeys;
using static SecuredApi.Testing.Common.Jwt.TokenHelper;

namespace SecuredApi.ComponentTests.Gateway;

public class OAuthTests: GatewayTestsBase
{
    public OAuthTests()
        :base("appsettings-oauth.json", (srv, config) =>
        {
            var keyProvider = Substitute.For<ISigningKeysProvider>();

            keyProvider.GetKeysAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                   .Returns(MakePublicKeysList(TestKey1, TestKey2)
               );

            srv.AddSingleton(keyProvider);
        })
    {
    }

    [Fact]
    public async Task PrivateRoute_CallAlowedConsumerWithActions()
    {
        var token = CreateJwtToken(JwtClaims.AllowedEntraTokenIssuer,
                                    JwtClaims.AllowedEntraTokenAudience,
                                    TestKey2,  
                                    new[] { "EchoSrv.Read.All", "EchoSrv.Write.All" },
                                    DateTime.UtcNow,
                                    TimeSpan.FromHours(1));
        SetToken(token);
        Request.SetupGet(RoutePaths.PrivateOAuthRedirectWildcard);

        // setup RemouteCall response
        MainHttpHandler.When(HttpMethod.Get, AppSettingnsProtectedRemoteEndpoint)
            .Respond(
                        HttpStatusCode.OK,
                        new StringContent(InlineContent.PrivateRedirectWildcard)
                    );

        ExpectedResult.StatusCode = StatusCodes.Status200OK;
        ExpectedResult.Body = InlineContent.PrivateRedirectWildcard;
        ExpectedResult.AddHeaders(Headers.TextPlainUtf8ContentType);

        await ExecuteAsync();
    }

    [Fact]
    public async Task PrivateRoute_CallNotAllowed()
    {
        var token = CreateJwtToken(JwtClaims.AllowedEntraTokenIssuer,
                                    JwtClaims.AllowedEntraTokenAudience,
                                    TestKey2,
                                    new[] { "EchoSrv.Read.All" },
                                    DateTime.UtcNow,
                                    TimeSpan.FromHours(1));
        SetToken(token);
        Request.SetupGet(RoutePaths.PrivateOAuthNotAllowedWildcard);

        // setup RemouteCall response
        MainHttpHandler.When(HttpMethod.Get, AppSettingnsProtectedRemoteEndpoint)
            .Respond(
                        HttpStatusCode.OK,
                        new StringContent(InlineContent.PrivateRedirectWildcard)
                    );

        ExpectedResult.StatusCode = StatusCodes.Status403Forbidden;
        ExpectedResult.Body = InlineContent.AccessDenied;

        await ExecuteAsync();
    }

    [Fact]
    public async Task PrivateRoute_CallAlowedConsumerWithWrongKey()
    {
        var token = CreateJwtToken(JwtClaims.AllowedEntraTokenIssuer,
                                    JwtClaims.AllowedEntraTokenAudience,
                                    TestKey3,
                                    new[] { "EchoSrv.Read.All", "EchoSrv.Write.All" },
                                    DateTime.UtcNow,
                                    TimeSpan.FromHours(1));
        SetToken(token);
        Request.SetupGet(RoutePaths.PrivateOAuthRedirectWildcard);

        // setup RemouteCall response
        MainHttpHandler.When(HttpMethod.Get, AppSettingnsProtectedRemoteEndpoint)
            .Respond(
                        HttpStatusCode.OK,
                        new StringContent(InlineContent.PrivateRedirectWildcard)
                    );

        ExpectedResult.StatusCode = StatusCodes.Status401Unauthorized;
        ExpectedResult.Body = InlineContent.NotAuthorized;

        await ExecuteAsync();
    }

    private void SetToken(string token)
    {
        Request.Headers.Add(new(Headers.AuthorizationHeaderName, OAuthHeaderPrefix + token));
    }
}

