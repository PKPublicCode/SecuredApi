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
    }

    public const string GlobalsPublicEndpointPath = "https://mock_endpoint/api";
    public const string RoutingPublicRedirectWildcardPath = "/public/wildcard/redirect/";
}

