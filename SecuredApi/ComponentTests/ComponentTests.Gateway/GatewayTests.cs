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
using Microsoft.Extensions.DependencyInjection;
using SecuredApi.Logic.Routing.Utils;
using System.Net;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace SecuredApi.ComponentTests.Gateway;

public class GatewayTests: GatewayTestsBase
{
    [Fact]
    public async Task RemoteCall()
    {
        const string body = "Hello";
        const string TestHeaderName = "TestHeaderName";
        StringValues TestHeaderValue = "TestHeaderValue";
        Request.SetupGet(PublicRedirectPath);


        MainHttpHandler.When(HttpMethod.Get, PublicRedirectCallPath)
            .Respond(
                        HttpStatusCode.PaymentRequired,
                        NewPairs((TestHeaderName, TestHeaderValue[0])),
                        MediaTypeNames.Text.Plain,
                        body
                    );

        await ExecuteAsync();

        Response.StatusCode.Should().Be(StatusCodes.Status402PaymentRequired);
        Response.Headers.Should().BeEquivalentTo
                                 (
                                    ToArray
                                    (
                                       CommonResponseHeader,
                                       PublicRedirectResponseHeader,
                                       NewPair(TestHeaderName, TestHeaderValue)
                                    )
                                 );
        Response.Body.ToString().Should().NotBeSameAs(body);
    }
}

