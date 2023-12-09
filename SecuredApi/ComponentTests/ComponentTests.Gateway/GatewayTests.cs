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
using System.Net.Mime;
using System.Net;

namespace SecuredApi.ComponentTests.Gateway;

public class GatewayTests: GatewayTestsBase
{
    // Test-case step-by-step explanation:
    //   Gateway receives call with urlPath and TestRequestHeader
    //   Gateway executes root routeGroup.preRequestActions
    //     AddRequestHeader: adds Headers.RequestCommon to request
    //   Geteway executes actions for appropriate route (wrt urlPath - see ["title" : "Call to remote endpoint"] in routing json ):
    //     RemoteCall: executed to remote endpoint (mocked)
    //       Remote endpoint receives call with appropriate URL with headers: Headers.RequestCommon and TestRequestHeader
    //       Remote endpoint replies with Code.Accepted, body as plain text and headers: TestResponseHeader, Headers.ResponseSuppressPublicRedirect, body media type (implicitly)
    //     SetResponseHeader: adds Headers.ResponsePublicRedirect header
    //     SuppressResponseHeader: removes from response header Headers.ResponseSuppressPublicRedirect (that was sent by endpoint and received by RemoteCallAction
    //   Gateway executes root routeGroup.preRequestActions
    //     SetResponseHeader: adds Headers.ResponseCommon header to response
    //
    // So, result:
    //   StatusCode: Accepted
    //   Body: body sent by remote call
    //   Headers:
    //      Headers.ResponseCommon - added by root routeGroup.preRequestActions
    //      Headers.ResponsePublicRedirect - added by route action
    //      Headers.TextPlainUtf8ContentType - implicitly sent by mocked remote endpoint with content
    //      TestResponseHeader - explicitly sent by mocked remote endpoint
    [Theory]
    [InlineData(RoutePaths.PublicRemoteWildcardGet, "", _methodGet, _methodGet)]
    [InlineData(RoutePaths.PublicRemoteWildcardGet, "/internal/path", _methodGet, _methodGet)]
    [InlineData(RoutePaths.PublicRemoteWildcardGet, "/internal/path", _methodPost, _methodGet)]
    [InlineData(RoutePaths.PublicRemoteWildcardOriginal, "/arbitrary/path", _methodGet, _methodGet)]
    [InlineData(RoutePaths.PublicRemoteWildcardOriginal, "/arbitrary/path", _methodPost, _methodPost)]
    public async Task RemoteCall_Found(string basePath, string urlPath, string requestMethod, string expectedCallMethod)
    {
        const string body = "TestBody";
        HttpHeader TestRequestHeader = new("TestResponseHeaderName", "TestResponseHeaderValue");
        HttpHeader TestResponseHeader = new("TestResponseHeaderName", "TestResponseHeaderValue");

        // Simulate received http call by gateway
        Request.SetupMethod($"{basePath}/{urlPath}", requestMethod);
        Request.Headers.Add(TestRequestHeader);

        // setup RemouteCall response
        MainHttpHandler.When(new HttpMethod(expectedCallMethod), $"{GlobalsPublicRemoteEndpoint}{urlPath}")
            .WithHeaders(new[] { Headers.RequestCommon.AsMock(), TestRequestHeader.AsMock() })
            .Respond(
                        HttpStatusCode.Accepted,
                        new[] { TestResponseHeader.AsMock(), Headers.ResponseSuppressPublicRedirect.AsMock() },
                        MediaTypeNames.Text.Plain,
                        body
                    );

        // Expected result
        ExpectedResult.StatusCode = StatusCodes.Status202Accepted;
        ExpectedResult.AddHeaders(Headers.ResponseCommon, Headers.ResponsePublicRedirect, Headers.TextPlainUtf8ContentType, TestResponseHeader);
        ExpectedResult.Body = body;

        await ExecuteAsync();
    }

    //Can't find existing methods defined as const string that is required to be used in attributes
    private const string _methodGet = "get";
    private const string _methodPost = "post";
}

