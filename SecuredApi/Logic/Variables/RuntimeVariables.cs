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
using System.Diagnostics.CodeAnalysis;

namespace SecuredApi.Logic.Variables;

public class RuntimeVariables: IRuntimeVariables
{
    private readonly Dictionary<string, object> _variables = new ();

    public object GetVariable(string key)
    {
        if (!_variables.TryGetValue(key, out var result))
        {
            throw new InvalidExpressionException($"Unknown variable {key}");
        }
        return result;
    }

    public object GetVariable(ReadOnlySpan<char> key) => GetVariable(key.ToString());

    public void SetVariable(string key, object value) => _variables[key] = value;

    public bool TryGetVariable(string key, [MaybeNullWhen(false)] out object value)
    {
        return _variables.TryGetValue(key, out value);
    }

    public bool TryGetVariable(ReadOnlySpan<char> key, [MaybeNullWhen(false)] out object value)
    {
        return _variables.TryGetValue(key.ToString(), out value);
    }
}

