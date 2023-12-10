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
using static SecuredApi.Logic.Routing.Variables.Constants.Runtime;
namespace SecuredApi.Logic.Routing.Variables;

public class RuntimeExpressionParser: IRuntimeExpressionParser
{
    private readonly ExpressionParser<ExpressionBuilder> _expressionParser;

    public RuntimeExpressionParser()
    {
        _expressionParser = new (_variableStart, _variableEnd, new BuilderFactory());
    }

    public RuntimeExpression Parse(string expression)
    {
        if (_expressionParser.Parse(expression, out var builder))
        {
            return builder.BuildExpression();
        }
        return ExpressionBuilder.MakeShortExpression(expression);
    }

    private class BuilderFactory : IExpressionBuilderFactory<ExpressionBuilder>
    {
        public ExpressionBuilder Create(int capacity) => new ExpressionBuilder(capacity);
    }

    private class ExpressionBuilder : IExpressionBuilder
    {
        private readonly List<ExpressionPart> _parts;

        public static RuntimeExpression MakeShortExpression(string expression)
        {
            return new RuntimeExpression(expression);
        }

        public ExpressionBuilder(int capacity)
        {
            _parts = new(capacity);
        }

        public RuntimeExpression BuildExpression() => new RuntimeExpression(_parts);

        public void AddPart(ReadOnlySpan<char> part, string _)
            => AddPart(part, false);

        public void AddVariable(ReadOnlySpan<char> variable, string _)
            => AddPart(variable, true);

        private void AddPart(ReadOnlySpan<char> part, bool isVariable)
        {
            _parts.Add(new ExpressionPart()
            {
                Value = part.ToString(),
                IsVariable = isVariable
            });
        }
    }
}

