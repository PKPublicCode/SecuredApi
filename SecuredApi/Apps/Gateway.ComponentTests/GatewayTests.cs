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
using RichardSzalay.MockHttp;
using System.Net.Mime;
using System.Net;
using Microsoft.AspNetCore.Http;
using static SecuredApi.Apps.Gateway.ComponentTests.Utils.Constants.RoutePaths;

namespace SecuredApi.Apps.Gateway.ComponentTests;

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
    [InlineData($"{PublicRemoteWildcardGet}/internal/path", "?myParam=6", _methodGet, $"{GlobalsPublicRemoteEndpoint}", "", _methodGet)]
    [InlineData($"{PublicRemoteWildcardGet}/internal/path", "", _methodPost, $"{GlobalsPublicRemoteEndpoint}", "", _methodGet)]
    [InlineData($"{PublicRemoteWildcardOriginal}", "", _methodGet, $"{GlobalsPublicRemoteEndpointWithExtra}", "", _methodGet)]
    [InlineData($"{PublicRemoteWildcardOriginal}/arbitrary/path", "", _methodGet, $"{GlobalsPublicRemoteEndpointWithExtra}arbitrary/path", "", _methodGet)]
    [InlineData($"{PublicRemoteWildcardOriginal}/arbitrary/path", "", _methodPost, $"{GlobalsPublicRemoteEndpointWithExtra}arbitrary/path", "", _methodPost)]
    [InlineData($"{PublicRemoteWildcardOriginal}/arbitrary/path", "?myParam=1&myParam=2", _methodGet, $"{GlobalsPublicRemoteEndpointWithExtra}arbitrary/path", "?myParam=1&myParam=2", _methodGet)]
    public async Task RemoteCallRouteWithWildcard_CallWithDifferentPaths_RemoteCalledWithPathEnding(string path, string query, string requestMethod, string exprectedPath, string expectedQuery, string expectedMethod)
    {
        const string body = "TestBody";
        HttpHeader TestRequestHeader = new("TestResponseHeaderName", "TestResponseHeaderValue");
        HttpHeader TestResponseHeader = new("TestResponseHeaderName", "TestResponseHeaderValue");

        // Simulate received http call by gateway
        Request.SetupMethod(path, requestMethod);
        Request.Headers.Add(TestRequestHeader);
        Request.QueryString = new QueryString(query);

        // setup RemouteCall response
        MainHttpHandler.When(new HttpMethod(expectedMethod), exprectedPath)
            .WithHeaders(new[] { Headers.RequestCommon.AsMock(), TestRequestHeader.AsMock() })
            .WithQueryString(expectedQuery)
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
    
    [Fact]
    public async Task RemoteCallRouteWithWildcard_CallWithSpecificParameters_RemoteCalledWithParameters()
    {
        const string body = "TestBody";

        // Simulate received http call by gateway
        Request.SetupMethod($"{PublicRemoteWildcardQueryParameters}", _methodGet);
        Request.QueryString = new QueryString("?param3=30&param2=20&param1=10");

        // setup RemouteCall response
        MainHttpHandler.When(new HttpMethod(_methodGet), 
                            $"{GlobalsPublicRemoteEndpointWithExtra}_b_newParam2_e_20_s_newParam3_e_30")
            .WithHeaders(new[] { Headers.RequestCommon.AsMock() })
            .WithQueryString("?newParam1=10&newParam2=20")
            .Respond(
                        HttpStatusCode.Accepted,
                        new KeyValuePair<string, string>[] { },
                        MediaTypeNames.Text.Plain,
                        body
                    );

        // Expected result
        ExpectedResult.StatusCode = StatusCodes.Status202Accepted;
        ExpectedResult.AddHeaders(Headers.ResponseCommon, Headers.TextPlainUtf8ContentType );
        ExpectedResult.Body = body;

        await ExecuteAsync();
    }

    //Can't find existing methods defined as const string that is required to be used in attributes
    private const string _methodGet = "get";
    private const string _methodPost = "post";
}

