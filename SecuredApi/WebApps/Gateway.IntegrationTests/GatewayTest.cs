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
using SecuredApi.WebApps.Gateway.Hosting;
using static SecuredApi.WebApps.Gateway.Utils.Constants.RoutePaths;
using System.Net;

namespace SecuredApi.WebApps.Gateway;

public class GatewayTests: TestsBase, IClassFixture<GatewayHostFixture>
{
    private GatewayHostFixture _gateway;

    public GatewayTests(GatewayHostFixture fixture)
        : base(fixture)
    {
        _gateway = fixture;
    }

    [Fact]
    public async Task ApiKeyCall_Positive()
    {
        Request.SetPost()
            .SetStringContent("Hello hello")
            .SetRelativePath(PrivateApiKeyRedirectWildcard)
            .AddHeader(Headers.SubscriptionKeyHeaderName, "5F39D492-A141-498A-AE04-76C6B77F246A");

        ExpectedResult.Body = InlineContent.EchoDelay;
        ExpectedResult.StatusCode = HttpStatusCode.OK;
        ExpectedResult.AddHeaders(Headers.ResponseConsumerSpecificActions, Headers.ResponseEchoServerRequestProcessed);

        await ActAsync();
        await AssertAsync();
    }

    [Fact]
    public async Task ApiKeyCall_NotAuthorized()
    {
        Request.SetPost()
            .SetStringContent("Hello hello")
            .SetRelativePath(PrivateApiKeyRedirectWildcard)
            .AddHeader(Headers.SubscriptionKeyHeaderName, "5F39D492-A141-498A-AE04-76C6B77F2463");

        ExpectedResult.StatusCode = HttpStatusCode.Unauthorized;
        ExpectedResult.AddHeaders(Headers.ResponseCommonOnError);
        ExpectedResult.Body = InlineContent.SubscriptionKeyNotSetOrInvalid;

        await ActAsync();
        await AssertAsync();
    }
}

