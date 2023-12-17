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
using static SecuredApi.Logic.Variables.Constants.Global;

namespace SecuredApi.Logic.Variables;

public class GlobalExpressionProcessor : IGlobalExpressionProcessor
{
    private readonly ExpressionParser<StringBuilderExpression> _expressionParser;

    public GlobalExpressionProcessor(IGlobalVariables globalVariables)
    {
        _expressionParser = new (_variableStart, _variableEnd, new BuilderFactory(globalVariables));
    }

    public string Parse(string expression)
    {
        if (_expressionParser.Parse(expression, out var sb))
        {
            return sb.ToString()!;
        }
        return expression;
    }

    private class BuilderFactory: IExpressionBuilderFactory<StringBuilderExpression>
    {
        private readonly IGlobalVariables _globalVariables;
        
        public BuilderFactory(IGlobalVariables globalVariables)
        {
            _globalVariables = globalVariables;
        }

        public StringBuilderExpression Create(int capacity)
            => new StringBuilderExpression(_globalVariables, capacity);
    }

    private class StringBuilderExpression: IExpressionBuilder
    {
        private readonly StringBuilder _sb;
        private readonly IGlobalVariables _globalVariables;

        public StringBuilderExpression(IGlobalVariables globalVariables, int capacity)
        {
            _globalVariables = globalVariables;
            _sb = new(capacity);
        }

        public void AddPart(ReadOnlySpan<char> part, string _) => _sb.Append(part);

        public void AddVariable(ReadOnlySpan<char> variable, string expression)
        {
            if (!_globalVariables.TryGetVariable(variable, out var value))
            {
                throw new InvalidExpressionException($"Variable {variable.ToString()} is not defined in {expression}");
            }
            _sb.Append(value);
        }

        public override string ToString() => _sb.ToString();
    }
}
