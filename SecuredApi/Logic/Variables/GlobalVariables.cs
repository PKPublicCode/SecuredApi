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

public class GlobalVariables : IGlobalVariables, IGlobalVariablesUpdater
{
    private Dictionary<string, string> _variables = new(StringComparer.OrdinalIgnoreCase);

    public string GetVariable(string key) => _variables[key];

    public string GetVariable(ReadOnlySpan<char> key) => _variables[key.ToString()];

    public bool TryGetVariable(string key, [MaybeNullWhen(false)] out string value)
    {
        return _variables.TryGetValue(key, out value);
    }

    public bool TryGetVariable(ReadOnlySpan<char> key, [MaybeNullWhen(false)] out string value)
    {
        return TryGetVariable(key.ToString(), out value);
    }

    public void Update(IEnumerable<KeyValuePair<string, string>> values)
    {
        var variables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach(var p in values)
        {
            variables.Add(p.Key, p.Value);
        }
        _variables = variables;
    }
}
