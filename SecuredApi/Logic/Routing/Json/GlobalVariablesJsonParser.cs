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
using System.Text.Json;

namespace SecuredApi.Logic.Routing.Json;

public class GlobalVariablesJsonParser : IGlobalVariablesStreamParser
{
    public async Task<GlobalConfiguration> ParseAsync(Stream s, CancellationToken cancellationToken)
    {
        var jsonDef =  await JsonSerializer.DeserializeAsync<JsonGlobalConfig>(s, _defaultSerializerOptions, cancellationToken)
           ?? throw new RouteConfigurationException("Unable to parse global configuration");

        return new GlobalConfiguration()
        {
            Variables = jsonDef.Variables?.Select(x => new KeyValuePair<string, string>(x.Name, x.Value)).ToList()
                ?? throw new RouteConfigurationException("Variables property in global config is not defined")
        };
    }

    private static JsonSerializerOptions _defaultSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    private class JsonVariableDefinition
    {
        public string Name { get; init; } = string.Empty;
        public string Value { get; init;  } = string.Empty;
    }

    private class JsonGlobalConfig
    {
        public List<JsonVariableDefinition>? Variables { get; init; }
    }
}
