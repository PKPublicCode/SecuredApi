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
using Microsoft.Extensions.Configuration;

namespace SecuredApi.Testing.Common;

public static class Configuration
{
    private const string _jsonExtension = ".json";

    public static IConfiguration Build(string fileBase, string? prefix = null)
    {
        var builder = new ConfigurationBuilder()
                    .AddJsonFile(fileBase + _jsonExtension, false)
                    .AddJsonFile(fileBase + "." + Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") + _jsonExtension, true)
                    .AddEnvironmentVariables(prefix);
        return builder.Build();
    }
}

