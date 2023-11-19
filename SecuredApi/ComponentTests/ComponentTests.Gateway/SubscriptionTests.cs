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
using Microsoft.AspNetCore.Http;
using RichardSzalay.MockHttp;
using System.Net;

namespace SecuredApi.ComponentTests.Gateway;

public class SubscriptionTests : GatewayTestsBase
{
    public SubscriptionTests()
    {
    }

    [Fact]
    public async Task PrivateRote_SubscriptionKeyNotSet()
    {
        Request.SetupGet(RoutePaths.PrivateRedirectWildcard);

        ExpectedResult.StatusCode = StatusCodes.Status401Unauthorized;
        ExpectedResult.Body = InlineContent.SubscriptionKeyNotSetOrInvalid;
        ExpectedResult.AddHeaders(Headers.ResponseCommonOnError);

        await ExecuteAsync();
    }

    [Fact]
    public async Task PrivateRote_SubscriptionKeyNotExists()
    {
        Request.SetupGet(RoutePaths.PrivateRedirectWildcard);
        SetSubscriptionKey("KeyKeyKey");

        ExpectedResult.StatusCode = StatusCodes.Status401Unauthorized;
        ExpectedResult.Body = InlineContent.SubscriptionKeyNotSetOrInvalid;
        ExpectedResult.AddHeaders(Headers.ResponseCommonOnError);

        await ExecuteAsync();
    }

    [Fact]
    public async Task PrivateRote_CallAlowedConsumerWithActions()
    {
        //Test uses same route as used in itegration tests, and so it has remoute call inside
        Request.SetupGet(RoutePaths.PrivateRedirectWildcard);
        SetSubscriptionKey("5F39D492-A141-498A-AE04-76C6B77F246A");

        // setup RemouteCall response
        MainHttpHandler.When(HttpMethod.Get, AppSettingnsProtectedRemoteEndpoint)
            .Respond(
                        HttpStatusCode.OK,
                        new StringContent(InlineContent.PrivateRedirectWildcard)
                    );

        ExpectedResult.StatusCode = StatusCodes.Status200OK;
        ExpectedResult.Body = InlineContent.PrivateRedirectWildcard;
        ExpectedResult.AddHeaders(Headers.ResponseCommon, Headers.ResponseConsumerSpecificActions, Headers.TextPlainUtf8ContentType);

        await ExecuteAsync();
    }

    [Fact]
    public async Task PrivateRote_CallNotAlowedConsumerWithActions()
    {
        Request.SetupGet(RoutePaths.PrivateNotAllowedWildcard);
        SetSubscriptionKey("5F39D492-A141-498A-AE04-76C6B77F246A");

        ExpectedResult.StatusCode = StatusCodes.Status403Forbidden;
        ExpectedResult.Body = InlineContent.CallNotAllowed;
        ExpectedResult.AddHeaders(Headers.ResponseCommonOnError);

        await ExecuteAsync();
    }

    private void SetSubscriptionKey(string key)
    {
        Request.Headers.Add(new(Headers.SubscriptionKeyHeaderName, key));
    }
}

