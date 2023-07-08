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
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using RichardSzalay.MockHttp;
using System.Net.Mime;
using System.Net;

namespace SecuredApi.ComponentTests.Gateway;

public class GatewayTests: GatewayTestsBase
{
    [Theory]
    [InlineData("")]
    [InlineData("/internal/path")]
    public async Task RemoteCall_Found(string urlPath)
    {
        const string body = "TestBody";
        HttpHeader testHeader = new("TestHeaderName", "TestHeaderValue");

        Request.SetupGet($"{RoutingPublicRemoteWildcardPath}/{urlPath}");

        MainHttpHandler.When(HttpMethod.Get, $"{GlobalsPublicRemoteEndpoint}{urlPath}")
            .WithHeaders(MakeArray(Headers.RequestCommon.AsMock()))
            .Respond(
                        HttpStatusCode.Accepted,
                        MakeArray(testHeader.AsMock()),
                        MediaTypeNames.Text.Plain,
                        body
                    );
        
        await ExecuteAsync();

        Response.StatusCode.Should().Be(StatusCodes.Status202Accepted);
        Response.Headers.Should().BeEquivalentTo
                                 (
                                    MakeArray
                                    (
                                       Headers.ResponseCommon,
                                       Headers.ResponsePublicRedirect,
                                       testHeader
                                    )
                                 );
        ResponseBody.Should().Be(body);
    }
}

