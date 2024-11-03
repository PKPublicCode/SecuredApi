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
using Functions = SecuredApi.Logic.Routing.Model.Functions.Request;
using Vars = SecuredApi.Logic.Routing.Model.RuntimeVariables.Request;

namespace SecuredApi.Logic.Routing.RuntimeExpressions;

// Ugly temporary implementation. Will be reimplemented if expressions will be changed to more complex language
public class RuntimeExpressionPartFactory: IRuntimeExpressionPartFactory
{
    public IRuntimeExpression CreatePart(ReadOnlySpan<char> expression, bool isVariable)
    {
        var expressionStr = expression.ToString();
        if (!isVariable)
        {
            return new StringPart(expressionStr);
        }

        // Try to find singleton expressions
        if(_simpleExpressions.TryGetValue(expressionStr, out var singleton))
        {
            return singleton;
        }

        if (TrySplit(expressionStr, out var pair))
        {
            switch (pair.Func)
            {
                case Functions.GetQueryParam:
                    return new GetRequestQueryParamPart(pair.Param);
                case Functions.GetVariable:
                    return new GetRuntimeVariablePart(pair.Param);
            }    
        }

        throw new InvalidOperationException($"Unable to parse expression {expressionStr}");
    }

    private static bool TrySplit(string expr, out (string Func, string Param) result)
    {
        int start = expr.IndexOf('(');
        if (start > 0
            && expr.Length > 0 && expr[^1] == ')')
        {
            result = (expr[..start], expr[(start + 1)..^2]);
            return true;
        }
        result = default;
        return false;
    }

    private static readonly Dictionary<string, IRuntimeExpression> _simpleExpressions = new()
    {
        { Functions.GetHttpMethod + "()", new GetRequestMethodPart() },
        { Functions.GetQueryString + "()", new GetRequestQueryStringPart() },
        { Functions.GetRemainingPath + "()", new GetRuntimeVariablePart(Vars.RemainingPath) },
    };
} 

