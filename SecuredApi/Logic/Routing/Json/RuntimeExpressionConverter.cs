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
using System.Text.Json.Serialization;

namespace SecuredApi.Logic.Routing.Json;

public class RuntimeExpressionConverter : JsonConverter<RuntimeExpression>
{
    private readonly IRuntimeExpressionParser _runtimeParser;

    public RuntimeExpressionConverter(IRuntimeExpressionParser runtimeParser)
    {
        _runtimeParser = runtimeParser;
    }

    public override RuntimeExpression Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = JsonSerializer.Deserialize<string>(ref reader, options)
            ?? throw new RouteConfigurationException("RuntimeExpression value can't be null");
        return _runtimeParser.Parse(value);
    }

    public override void Write(Utf8JsonWriter writer, RuntimeExpression value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
