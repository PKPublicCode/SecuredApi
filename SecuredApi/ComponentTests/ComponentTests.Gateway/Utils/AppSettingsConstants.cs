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
using Microsoft.Net.Http.Headers;
using System.Net.Mime;

namespace SecuredApi.ComponentTests.Gateway.Utils;

// Represents strings used in routing.json and globals.json
public static class AppSettingsConstants
{
    public static class Headers
    {
        public static HttpHeader RequestCommon { get; } = new("X-COMMON-REQUEST-HEADER", "CommonRequestHeaderValue");

        public static HttpHeader ResponseCommon { get; } = new("X-COMMON-RESPONSE-HEADER", "CommonResponseHeaderValue");

        public static HttpHeader ResponseNotFound { get; } = new("X-NOTFOUND-RESPONSE-HEADER", "NotFoundResponseHeaderValue");

        public static HttpHeader ResponsePublicRedirect { get; } = new("X-PUBLIC-REDIRECT-RESPONSE-HEADER", "PublicRedirectResponseHeaderValue");

        public static HttpHeader ResponseSuppressPublicRedirect { get; } = new("X-PUBLIC-REDIRECT-SUPPRESS-HEADER", "Blablabla");

        public static HttpHeader TextHtmlContentType { get; } = new(HeaderNames.ContentType, MediaTypeNames.Text.Html);

        public static HttpHeader TextPlainUtf8ContentType { get; } = new(HeaderNames.ContentType, $"{MediaTypeNames.Text.Plain}; charset=utf-8");
    }

    public const string GlobalsPublicRemoteEndpoint = "https://remote.endpoint/api";

    public static class RoutePaths
    {
        public const string PublicRemoteWildcard = "/public/remote/wildcard";
        public const string PublicEchoWildcard = "/public/echo/wildcard";
        public const string PublicEchoExact = "/public/echo/exact";
        public const string PublicEchoDelay = "/public/echo/delay";
        public const string PublicContentBase = "/public/content";
    }

    public static class InlineContent
    {
        public const string ResponseEchoExact = "Echo exact route";
        public const string ResponseEchoWildcard = "Echo wildcard route";
        public const string ResponseNotFound = "Route not found";
        public const string ResponseStaticFileWildcardNotFound = "This is wrong file url";
        public const string ResponseEchoDelay = "Public delay route";
    }

    public static IPAddress EchoWildcardAllowedIp { get; } = IPAddress.Parse("20.20.20.21");
    public const int PublicEchoDelayMilliseconds = 300;

    public static class PublicContent
    {
        public static class Exact 
        {
            public const string Path = "/Exact";
            public const string Content =
@"<html>
<head>
    <meta charset=""utf-8"" />
    <title>Secured API</title>
</head>
<body>
    <h1>Welcome to Secured API</h1>
    This is default configuration.<br>
    Enjoy!!!
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

