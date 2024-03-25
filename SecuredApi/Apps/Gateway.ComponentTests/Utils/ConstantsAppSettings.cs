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
using System.Net;

namespace SecuredApi.Apps.Gateway.ComponentTests.Utils;

// Represents strings used in routing.json and globals.json
public static partial class Constants
{
    public static partial class Headers
    {
        public static HttpHeader RequestCommon { get; } = new("X-COMMON-REQUEST-HEADER", "CommonRequestHeaderValue");

        public static HttpHeader ResponseCommon { get; } = new("X-COMMON-RESPONSE-HEADER", "CommonResponseHeaderValue");

        public static HttpHeader ResponseNotFound { get; } = new("X-NOTFOUND-RESPONSE-HEADER", "NotFoundResponseHeaderValue");

        public static HttpHeader ResponsePublicRedirect { get; } = new("X-PUBLIC-REDIRECT-RESPONSE-HEADER", "PublicRedirectResponseHeaderValue");

        public static HttpHeader ResponseSuppressPublicRedirect { get; } = new("X-PUBLIC-REDIRECT-SUPPRESS-HEADER", "Blablabla");

        public static HttpHeader ResponseCommonOnError { get; } = new("X-COMMON-ON-ERROR-HEADER", "Gateway Rejected Your Call");

        public static HttpHeader ResponseConsumerSpecificActions { get; } = new("X-CONSUMER-SPECIFIC-HEADER", "This is test client 1");

        public const string SubscriptionKeyHeaderName = "X-SUBSCRIPTION-KEY";

        public const string AuthorizationHeaderName = "Authorization";
    }

    public static class RoutePaths
    {
        public const string PublicRemoteWildcardGet = "/public/remote/wildcard_get_method";
        public const string PublicRemoteWildcardOriginal = "/public/remote/wildcard_original_method";
        public const string PublicEchoWildcard = "/public/echo/wildcard";
        public const string PublicEchoExact = "/public/echo/exact";
        public const string PublicEchoDelay = "/echo/delay";
        public const string PublicContentBase = "/public/echo/content";
        public const string PrivateApiKeyRedirectWildcard = "/private_api_key/redirect/wildcard";
        public const string PrivateApiKeyNotAllowedWildcard = "/private_api_key/notallowed/wildcard";
        public const string PrivateJwtRedirectWildcard = "/private_jwt/redirect/wildcard";
        public const string PrivateJwtNotAllowedWildcard = "/private_jwt/notallowed/wildcard";
    }

    public static class InlineContent
    {
        public const string EchoExact = "Echo exact route";
        public const string EchoWildcard = "Echo wildcard route";
        public const string NotFound = "Route not found";
        public const string StaticFileWildcardNotFound = "This is wrong file url";
        public const string EchoDelay = "This is Echo!\n V0.0001\n, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\n";
        public const string SubscriptionKeyNotSetOrInvalid = "Subscription key not set or invalid";
        public const string CallNotAllowed = "Call Not Allowed";
        public const string PrivateRedirectWildcard = "Private echo wildcard route, api 1";
        public const string NotAuthorized = "Not Authorized";
        public const string AccessDenied = "Access Denied";
    }

    public static class JwtClaims
    {
        public const string AllowedEntraTokenAudience = "api://secured-api-gateway";
        public const string AllowedEntraTokenIssuer = "https://secured-api.com/";
    }

    public const string GlobalsPublicRemoteEndpoint = "https://remote.endpoint/api";
    public const string GlobalsPublicRemoteEndpointWithExtra = "https://remote.endpoint/api/new_remote_path/";
    public const string AppSettingnsProtectedRemoteEndpoint = "https://protected.remote.endpoint/api.v2/";
    public static IPAddress EchoWildcardAllowedIp { get; } = IPAddress.Parse("20.20.20.21");
    public const int PublicEchoDelayMilliseconds = 300;
    public const string JwtHeaderPrefix = "Bearer ";

    public static class PublicContent
    {
        public static class Exact 
        {
            public const string Path = "/Exact";
            public const string Content =
@"<html>
<head>
    <meta charset=""utf-8"" />
    <title>Secured API test instance</title>
</head>
<body>
    <h1>Welcome to Secured API</h1>
    This is test configuration<br>
</body>
</html>";
        }

        public static class WildcardHelloTxt
        {
            public const string Path = "/wildcard/Hello.txt";
            public const string Content = "This is content of hello.txt";
        }
    }
}

