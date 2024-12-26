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
using static SecuredApi.Logic.Variables.Constants.Runtime;
using SecuredApi.Logic.Variables;

namespace SecuredApi.Logic.Routing.RuntimeExpressions;

public class RuntimeExpressionParser: IRuntimeExpressionParser
{
    private readonly ExpressionParser<ExpressionBuilder> _expressionParser;

    public RuntimeExpressionParser(IRuntimeExpressionPartFactory partFactory)
    {
        _expressionParser = new (_variableStart, _variableEnd, new BuilderFactory(partFactory));
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
        private readonly IRuntimeExpressionPartFactory _partFactory;
        public BuilderFactory(IRuntimeExpressionPartFactory partFactory)
        {
            _partFactory = partFactory;
        }

        public ExpressionBuilder Create(int capacity) => new ExpressionBuilder(capacity, _partFactory);
    }

    private class ExpressionBuilder : IExpressionBuilder
    {
        private readonly List<IRuntimeExpression> _parts;
        IRuntimeExpressionPartFactory _partFactory;

        public static RuntimeExpression MakeShortExpression(string expression)
        {
            return new RuntimeExpression(expression);
        }

        public ExpressionBuilder(int capacity, IRuntimeExpressionPartFactory partFactory)
        {
            _parts = new(capacity);
            _partFactory = partFactory;
        }

        public RuntimeExpression BuildExpression()
        {
            return new RuntimeExpression(new ComplexRuntimeExpression(_parts));
        }

        public void AddPart(ReadOnlySpan<char> part, string _)
            => AddPart(part, false);

        public void AddVariable(ReadOnlySpan<char> variable, string _)
            => AddPart(variable, true);

        private void AddPart(ReadOnlySpan<char> part, bool isVariable)
        {
            _parts.Add(_partFactory.CreatePart(part, isVariable));
        }
    }
}

