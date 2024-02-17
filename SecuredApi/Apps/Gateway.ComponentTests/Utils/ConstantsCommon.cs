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

using Microsoft.Net.Http.Headers;
using System.Net.Mime;

namespace SecuredApi.ComponentTests.Gateway.Utils;

// Represents common HTTP related strings
public static partial class Constants
{
    public static partial class Headers
    {
        public static HttpHeader TextHtmlContentType { get; } = new(HeaderNames.ContentType, MediaTypeNames.Text.Html);

        public static HttpHeader TextPlainUtf8ContentType { get; } = new(HeaderNames.ContentType, $"{MediaTypeNames.Text.Plain}; charset=utf-8");
    }
}

