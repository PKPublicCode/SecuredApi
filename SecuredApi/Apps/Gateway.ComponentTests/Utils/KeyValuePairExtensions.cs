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

namespace SecuredApi.Apps.Gateway.ComponentTests.Utils;

public static class KeyValuePairExtensions
{
    // MockHttpMessageHandler uses <string, string> pair, but asp.net (e.g. HttpContext) uses <string, StringValues>
    // This shortcut lets using unified instanses (constants) for both classes
    public static KeyValuePair<string, string> AsMock(this HttpHeader pair) => new(pair.Key, pair.Value[0] ?? string.Empty);
}

