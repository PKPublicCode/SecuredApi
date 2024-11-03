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
namespace SecuredApi.Logic.Routing.RuntimeExpressions;

public class GetRuntimeVariablePart: IRuntimeExpression
{
    private readonly string _value;

    public GetRuntimeVariablePart(string value)
    {
        _value = value;
    }

    public string Evaluate(IRequestContext ctx)
    {
        string? result = null;
        if (ctx.Variables.TryGetVariable(_value, out var value)
            && value is string)
        {
            result = value.ToString();
        }
        return result
                ?? throw new InvalidOperationException($"Variable {_value} is null");
    }
}

