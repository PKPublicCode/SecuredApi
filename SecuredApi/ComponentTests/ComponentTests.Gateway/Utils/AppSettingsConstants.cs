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
using Microsoft.Extensions.Primitives;

namespace SecuredApi.ComponentTests.Gateway.Utils;

public static class AppSettingsConstants
{
    public const string CommonRequestHeaderName = "X-COMMON-REQUEST-HEADER";
    public const string CommonRequestHeaderValue = "CommonRequestHeaderValue";
    public static readonly KeyValuePair<string, StringValues> CommonRequestHeader
                                    = new (CommonRequestHeaderName, CommonRequestHeaderValue);

    public const string CommonResponseHeaderName = "X-COMMON-RESPONSE-HEADER";
    public const string CommonResponseHeaderValue = "CommonResponseHeaderValue";
    public static readonly KeyValuePair<string, StringValues> CommonResponseHeader
                                    = new(CommonResponseHeaderName, CommonResponseHeaderValue);

    public const string NotFoundResponseHeaderName = "X-NOTFOUND-RESPONSE-HEADER";
    public const string NotFoundResponseHeaderValue = "NotFoundResponseHeaderValue";
    public static readonly KeyValuePair<string, StringValues> NotFoundResponseHeader
                                    = new(NotFoundResponseHeaderName, NotFoundResponseHeaderValue);

    public const string PublicRedirectCallPath = "https://mock_path/public/redirect";
    public const string PublicRedirectPath = "/public/redirect";

    public const string PublicRedirectResponseHeaderName = "X-PUBLIC-REDIRECT-RESPONSE-HEADER";
    public const string PublicRedirectResponseHeaderValue = "PublicRedirectResponseHeaderValue";
    public static readonly KeyValuePair<string, StringValues> PublicRedirectResponseHeader
                                    = new(PublicRedirectResponseHeaderName, PublicRedirectResponseHeaderValue);
}

