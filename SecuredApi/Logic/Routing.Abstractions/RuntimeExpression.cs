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
using System.Text;
namespace SecuredApi.Logic.Routing;

public readonly struct RuntimeExpression
{
    private readonly List<ExpressionPart> _parts;

    public RuntimeExpression(List<ExpressionPart> parts)
    {
        _parts = parts;
    }

    public string BuildString(IRuntimeVariables variables)
    {
        // Optimization. If only one element, that we don't need to create extra objects
        if (_parts.Count == 1)
        {
            var e = _parts[0];
            if (e.IsVariable)
            {
                return variables.GetVariable(e.Value).ToString()
                    ?? throw new InvalidOperationException($"Variable {e.Value} is null");
            }
            return e.Value;
        }

        var sb = new StringBuilder();
        foreach(var p in _parts)
        {
            if(p.IsVariable)
            {
                sb.Append(variables.GetVariable(p.Value));
            }
            else
            {
                sb.Append(p.Value);
            }
        }
        return sb.ToString();
    }
}

public readonly struct ExpressionPart
{
    public string Value { get; init; }
    public bool IsVariable { get; init; }
}

