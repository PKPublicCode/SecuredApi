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
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SecuredApi.Logic.Routing.Json
{
    public class StringExpressionConverter : JsonConverter<string>
    {
        private readonly JsonSerializerOptions _options;
        private readonly IExpressionProcessor _processor;

        public StringExpressionConverter(JsonSerializerOptions defaultOptions, IExpressionProcessor processor)
        {
            _options = defaultOptions;
            _processor = processor;
        }

        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = JsonSerializer.Deserialize<string>(ref reader, _options);
            if (value != null)
            {
                return _processor.ConvertExpression(value);
            }
            return value;
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}