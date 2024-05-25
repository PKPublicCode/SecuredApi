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
using Microsoft.AspNetCore.Http;
using static SecuredApi.Testing.Common.Jwt.SigningKeys;
using SecuredApi.Testing.Common.Jwt;
using Microsoft.Extensions.DependencyInjection;

namespace SecuredApi.Apps.Gateway.ComponentTests;

public class EntraJwtTests: GatewayTestsBase
{
    public EntraJwtTests()
        :base("appsettings-entrajwt.json", (srv, config) =>
        {
            var keyProvider = Substitute.For<ISigningKeysProvider>();

            keyProvider.GetKeysAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                   .Returns(TokenHelper.MakePublicKeysList(TestKey1, TestKey2)
               );

            srv.AddSingleton(keyProvider);
        })
    {
    }

    [Fact]
    public async Task RouteProtectedWithReadRole_CallWithReadRoleToken_Success()
    {
        var token = CreateJwtToken(JwtClaims.AllowedEntraTokenIssuer,
                                    JwtClaims.AllowedEntraTokenAudience,
                                    TestKey2,  
                                    new[] { "EchoSrv.Read.All", "EchoSrv.Write.All" },
                                    DateTime.UtcNow,
                                    TimeSpan.FromHours(1));
        SetToken(token);
        Request.SetupGet(RoutePaths.PrivateJwtRedirectWildcard);

        // setup RemouteCall response
        MainHttpHandler.When(HttpMethod.Get, AppSettingnsProtectedRemoteEndpoint)
            .Respond(
                        HttpStatusCode.OK,
                        new StringContent(InlineContent.PrivateRedirectWildcard)
                    );

        ExpectedResult.StatusCode = StatusCodes.Status200OK;
        ExpectedResult.Body = InlineContent.PrivateRedirectWildcard;
        ExpectedResult.AddHeaders(Headers.ResponseConsumerSpecificActions, Headers.TextPlainUtf8ContentType);

        await ExecuteAsync();
    }

    [Fact]
    public async Task RouteProtectedWithWriteRole_CallWithReadRoleToken_AccessDenied()
    {
        var token = CreateJwtToken(JwtClaims.AllowedEntraTokenIssuer,
                                    JwtClaims.AllowedEntraTokenAudience,
                                    TestKey2,
                                    new[] { "EchoSrv.Read.All" },
                                    DateTime.UtcNow,
                                    TimeSpan.FromHours(1));
        SetToken(token);
        Request.SetupGet(RoutePaths.PrivateJwtNotAllowedWildcard);

        // setup RemouteCall response
        MainHttpHandler.When(HttpMethod.Get, AppSettingnsProtectedRemoteEndpoint)
            .Respond(
                        HttpStatusCode.OK,
                        new StringContent(InlineContent.PrivateRedirectWildcard)
                    );

        ExpectedResult.StatusCode = StatusCodes.Status403Forbidden;
        ExpectedResult.Body = InlineContent.AccessDenied;
        ExpectedResult.AddHeaders(Headers.ResponseConsumerSpecificActions);

        await ExecuteAsync();
    }

    [Fact]
    public async Task RouteProtectedWithReadRole_CallWithTokenSignedByNotExpectedKey_NotAuthorized()
    {
        var token = CreateJwtToken(JwtClaims.AllowedEntraTokenIssuer,
                                    JwtClaims.AllowedEntraTokenAudience,
                                    TestKey3,
                                    new[] { "EchoSrv.Read.All", "EchoSrv.Write.All" },
                                    DateTime.UtcNow,
                                    TimeSpan.FromHours(1));
        SetToken(token);
        Request.SetupGet(RoutePaths.PrivateJwtRedirectWildcard);

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

    [Fact]
    public async Task RouteProtectedWithReadRole_CallWithMalformedToken_NotAuthorized()
    {
        SetToken("my token blablabla");
        Request.SetupGet(RoutePaths.PrivateJwtRedirectWildcard);

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
        Request.Headers.Add(new(Headers.AuthorizationHeaderName, JwtHeaderPrefix + token));
    }

    private static string CreateJwtToken(string issuer,
                                        string audience,
                                        RsaKeyInfo? key,
                                        IEnumerable<string> roles,
                                        DateTime start,
                                        TimeSpan duration,
                                        string? scope = null)
    {
        return TokenHelper.CreateJwtToken(JwtAppId, issuer, audience, key, roles, start, duration, scope);
    }
}

