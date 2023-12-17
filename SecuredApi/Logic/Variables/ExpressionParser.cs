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

public class ExpressionParser<T>
    where T: IExpressionBuilder
{
    private readonly string _variableStart;
    private readonly char _variableEnd;
    private readonly IExpressionBuilderFactory<T> _factory;

    public ExpressionParser(string startMarker, char endMarker, IExpressionBuilderFactory<T> factory)
    {
        _variableStart = startMarker;
        _variableEnd = endMarker;
        _factory = factory;
    }

    public bool Parse(string expression, [MaybeNullWhen(false)] out T builder)
    {
        int start = expression.IndexOf(_variableStart);
        if (start >= 0)
        {
            builder = _factory.Create(GetCapacity(start, expression));
            ProcessExpression(expression, builder, start);
            return true;
        }
        builder = default;
        return false;
    }

    private int GetCapacity(int index, string expression)
    {
        int capacity = index > 0 ? 2 : 1; //if symbols before the variable, then capacity at least 2
        int endIndex = expression.IndexOf(_variableEnd, index);
        if (endIndex == -1)
        {
            throw new InvalidExpressionException($"Invalid expression {expression}");
        }
        index = expression.IndexOf(_variableStart, endIndex);
        while (index != -1)
        {
            ++capacity; // found next variable
            if (endIndex + 1 != index)
            {
                ++capacity; //string between prev variable and new variable
            }
            endIndex = expression.IndexOf(_variableEnd, index);
            if (endIndex == -1)
            {
                throw new InvalidExpressionException($"Invalid expression {expression}");
            }
            index = expression.IndexOf(_variableStart, endIndex);
        }
        if (endIndex < expression.Length - 1)
        {
            ++capacity; //one more part in the end   
        }
        return capacity;
    }

    private void ProcessExpression(string expression, IExpressionBuilder sb, int varStart)
    {
        var expressionSpan = expression.AsSpan();
        int begin = 0;
        do
        {
            if (begin < varStart)
            {
                sb.AddPart(expressionSpan[begin..varStart], expression); //add part of expression before start
            }
            varStart += _variableStart.Length; //move to beginning of variable
            int varEnd = expression.IndexOf(_variableEnd, varStart); //find end of variable
            if (varEnd < 0)
            {
                throw new InvalidExpressionException($"Invalid expression {expression}");
            }
            sb.AddVariable(expressionSpan[varStart..varEnd], expression);
            begin = varEnd + 1; //move to the beginnig of remaining expression
            varStart = expression.IndexOf(_variableStart, begin); //find start of the variable
        }
        while (varStart > 0);
        if (begin < expression.Length)
        {
            sb.AddPart(expressionSpan[begin..expression.Length], expression);
        }
    }
}

