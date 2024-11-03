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
    private readonly IRuntimeExpression? expression;
    private readonly string? _immutableValue;

    public RuntimeExpression(IRuntimeExpression parts)
    {
        expression = parts;
    }

    public RuntimeExpression(string value)
    {
        _immutableValue = value;
    }

    public bool Immutable => _immutableValue != null;

    public string ImmutableValue => _immutableValue
        ?? throw new InvalidOperationException("Trying to access null value of expression. Use BuildString instead");

    public string BuildString(IRequestContext ctx)
    {
        // Optimization. Value is constant
        if (_immutableValue != null)
        {
            return _immutableValue;
        }

        return expression!.Evaluate(ctx); 
    }
}

