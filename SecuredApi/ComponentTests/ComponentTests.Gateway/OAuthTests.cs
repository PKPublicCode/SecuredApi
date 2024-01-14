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
using SecuredApi.Logic.Routing.Actions.OAuth;
using SecuredApi.Logic.Routing.Engine;
using SecuredApi.Logic.Auth.Jwt;
using RichardSzalay.MockHttp;
using static SecuredApi.Testing.Common.Jwt.SigningKeys;
using static SecuredApi.Testing.Common.Jwt.TokenHelper;
using System.Net;

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

    //[Fact]
    //public async Task TestAction()
    //{
    //    string stsDiscoveryEndpoint = "https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration";

    //    ConfigurationManager<OpenIdConnectConfiguration> configManager
    //        = new ConfigurationManager<OpenIdConnectConfiguration>(stsDiscoveryEndpoint, new OpenIdConnectConfigurationRetriever());

    //    OpenIdConnectConfiguration config = await configManager.GetConfigurationAsync();

    //    Assert.True(true);
    //}

    [Fact]
    public async Task PrivateRote_CallAlowedConsumerWithActions()
    {
        var token = CreateJwtToken(JwtClaims.AllowedEntraTokenIssuer,
                                    JwtClaims.AllowedEntraTokenAudience,
                                    TestKey2,
                                    MakeList("EchoSrv.Read.All"),
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

    private void SetToken(string token)
    {
        Request.Headers.Add(new("Authorization", "Bearer " + token));
    }

    [Fact]
    public async Task TestAction()
    {
        var issuer = "https://sts.windows.net/a9e2b040-93ef-4252-992e-0d9830029ae8/";
        var audience = "api://securedapi-gateway-ptst";
        var settings = new CheckEntraJwtActionSettings(
            OneOfIssuers: MakeList(issuer),
            OneOfAudiences: new[] { audience }
            );
        var sut = new CheckEntraJwtAction(settings);
        var token = CreateJwtToken(issuer, audience, TestKey2, Array.Empty<string>(), DateTime.UtcNow, TimeSpan.FromHours(1));

        Request.Headers.Add(new("Authorization", "Bearer " + token));

        using var scope = _serviceProvider.CreateAsyncScope();
        Context.RequestServices = scope.ServiceProvider;
        var ctx = new RequestContext(null!, Context);

        bool result = await sut.ExecuteAsync(ctx);

        Assert.True(result);
    }

    private static T[] MakeList<T>(params T[] a) => a;
}

