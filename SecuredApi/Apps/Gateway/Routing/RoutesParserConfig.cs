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
using SecuredApi.Logic.Routing;
using SecuredApi.Logic.Routing.Json;

namespace SecuredApi.Apps.Gateway.Routing;

public class RoutesParserConfig : IRoutesParserConfig
{
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly JsonDocumentOptions _documentOptions;

    public JsonSerializerOptions SerializerOptions => _serializerOptions;

    public JsonDocumentOptions DocumentOptions => _documentOptions;

    public RoutesParserConfig(IGlobalExpressionProcessor globalProcessor, IRuntimeExpressionParser runtimeParser)
    {
        _serializerOptions = new (CommonSerializerOptions.Instance);
        _serializerOptions.Converters.Add(new StringGlobalExpressionConverter(globalProcessor));
        _serializerOptions.Converters.Add(new RuntimeExpressionConverter(runtimeParser));

        _documentOptions = new ()
        {
            CommentHandling = CommonSerializerOptions.Instance.ReadCommentHandling
        };
    }
}
