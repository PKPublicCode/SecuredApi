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
    [InlineData($"{RoutePaths.PublicEchoExact}/path")]
    public async Task Echo_NotFound(string urlPath)
    {
        Request.SetupGet(urlPath);

        ExpectedResult.StatusCode = StatusCodes.Status404NotFound;
        ExpectedResult.AddHeaders(Headers.ResponseNotFound);
        ExpectedResult.Body = InlineContent.ResponseNotFound;

        await ExecuteAsync();
    }

    [Theory]
    [InlineData(RoutePaths.PublicEchoExact, InlineContent.ResponseEchoExact)]
    [InlineData($"{RoutePaths.PublicEchoWildcard}", InlineContent.ResponseEchoWildcard)]
    [InlineData($"{RoutePaths.PublicEchoWildcard}/", InlineContent.ResponseEchoWildcard)]
    [InlineData($"{RoutePaths.PublicEchoWildcard}/path", InlineContent.ResponseEchoWildcard)]
    public async Task EchoRoute_Found(string urlPath, string expectedContent)
    {
        Request.SetupGet(urlPath);

        ExpectedResult.StatusCode = StatusCodes.Status200OK;
        ExpectedResult.AddHeaders(Headers.ResponseCommon);
        ExpectedResult.Body = expectedContent;

        await ExecuteAsync();
    }

    [Theory]
    [InlineData(PublicContent.Exact.Path, PublicContent.Exact.Content)]
    public async Task StaticFile_Found(string urlPath, string expectedContent)
    {
        Request.SetupGet($"{RoutePaths.PublicContentBase}{urlPath}");

        ExpectedResult.StatusCode = StatusCodes.Status200OK;
        ExpectedResult.AddHeaders(Headers.ResponseCommon, Headers.TextHtmlContentType);
        ExpectedResult.Body = expectedContent;

        await ExecuteAsync();
    }
}

