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
namespace SecuredApi.WebApps.Gateway.Utils;

// Represents strings used in routing.json and globals.json
public static partial class Constants
{
    public static partial class Headers
    {
        public static HttpHeader ResponseCommonOnError { get; } = new("X-COMMON-ON-ERROR-HEADER", "Gateway Rejected Your Call");

        public static HttpHeader ResponseConsumerSpecificActions { get; } = new("X-CONSUMER-SPECIFIC-HEADER", "This is test client 1");

        public static HttpHeader ResponseEchoServerRequestProcessed { get; } = new("X-ECHO-SERVER", "Echo server processed request");

        public const string SubscriptionKeyHeaderName = "X-SUBSCRIPTION-KEY";

        public const string AuthorizationHeaderName = "Authorization";
    }

    public static class RoutePaths
    {
        public const string ApiKeyBasicFeatures = "/api/api_key/basic_features/delay";
        public const string ApiKeyPriviligedFeatures = "/api/api_key/privileged_features/delay";
        public const string ApiEntraJwtBasicFeatures = "/api/jwt/basic_features/delay";
        public const string ApiEntraJwtPreviligedFeatures = "/api/jwt/privileged_features/delay";
        public const string PublicStaticContent = "/ui";
    }

    public static class InlineContent
    {
        public const string EchoDelay = "This is Echo!\n V0.0001\n, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\n";
        public const string NotAuthorized = "Not Authorized";
        public const string AccessDenied = "Access Denied";
        public const string StaticFileNotFound = "";
    }

    public const string JwtHeaderPrefix = "Bearer ";

    public static class PublicContent
    {
        public static class WelcomeHtml 
        {
            public const string Path = "/welcome.html";
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
            public const string Path = "/Content/Hello.txt";
            public const string Content = "This is content of hello.txt";
        }
    }
}

