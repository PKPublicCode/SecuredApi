﻿// Copyright (c) 2021 - present, Pavlo Kruglov.
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
        public const string PrivateApiKeyRedirectWildcard = "/private_api_key/redirect/wildcard";
        public const string PrivateApiKeyNotAllowedWildcard = "/private_api_key/notallowed/wildcard";
        public const string PrivateOAuthRedirectWildcard = "/private_oauth/redirect/wildcard/delay";
        public const string PrivateOAuthNotAllowedWildcard = "/private_oauth/notallowed/wildcard";
    }

    public static class InlineContent
    {
        public const string EchoDelay = "This is Echo!\n V0.0001\n, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\nResponse, blabla blabla\nsome payload required to mimic valuable response from server. need data in the body.\n";
        public const string SubscriptionKeyNotSetOrInvalid = "Subscription key not set or invalid";
        public const string NotAuthorized = "Not Authorized";
        public const string AccessDenied = "Access Denied";
    }

    public const string OAuthHeaderPrefix = "Bearer ";

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

