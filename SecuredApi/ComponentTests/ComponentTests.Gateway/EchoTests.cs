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

namespace SecuredApi.ComponentTests.Gateway;

public class EchoTests: GatewayTestsBase
{
    public EchoTests()
    {
    }

    [Theory]
    [InlineData("/")]
    [InlineData($"{RoutingPublicEchoExactPath}/path")]
    public async Task Echo_NotFound(string urlPath)
    {
        Request.SetupGet(urlPath);

        await ExecuteAsync();

        Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        Response.Headers.Should().BeEquivalentTo(MakeArray(Headers.ResponseNotFound));
    }

    //[Fact]
    //public async Task RootRoute_NotFound()
    //{
    //    Request.SetupGet("/");

    //    await ExecuteAsync();

    //    Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    //    Response.Headers.Should().BeEquivalentTo(MakeArray(Headers.ResponseNotFound));
    //}

    //[Fact]
    //public async Task RouteWithExtraPath_NotFound()
    //{
    //    Request.SetupGet("/echo/success/extrapath");

    //    await ExecuteAsync();

    //    Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    //    Response.Headers.Should().BeEquivalentTo(MakeArray(Headers.ResponseNotFound));
    //}

    [Theory]
    [InlineData(RoutingPublicEchoExactPath)]
    [InlineData($"{RoutingPublicEchoWildcardPath}")]
    [InlineData($"{RoutingPublicEchoWildcardPath}/")]
    [InlineData($"{RoutingPublicEchoWildcardPath}/path")]
    public async Task EchoRoute_Found(string urlPath)
    {
        Request.SetupGet(urlPath);
        
        await ExecuteAsync();

        Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        Response.Headers.Should().BeEquivalentTo(MakeArray(Headers.ResponseCommon));
    }
}

