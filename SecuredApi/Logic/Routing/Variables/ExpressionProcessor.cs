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
using System;
using System.Text;

namespace SecuredApi.Logic.Routing.Variables
{
    public class ExpressionProcessor : IExpressionProcessor
    {
        private readonly IGlobalVariables _globalVariables;

        public ExpressionProcessor(IGlobalVariables globalVariables)
        {
            _globalVariables = globalVariables;
        }

        public string ConvertExpression(string expression)
        {
            int start = expression.IndexOf(_variableStart);
            if (start >= 0)
            {
                return ProcessExpression(expression, start);
            }
            return expression;
        }

        private string ProcessExpression(string expression, int varStart)
        {
            var sb = new StringBuilder();
            int begin = 0;
            do
            {
                sb.Append(expression.AsSpan()[begin..varStart]); //add part of expression before start
                varStart += _variableStart.Length; //move to beginning of variable
                int varEnd = expression.IndexOf(_variableEnd, varStart); //find end of variable
                if (varEnd < 0)
                {
                    throw new RouteConfigurationException($"Invalid expression {expression}");
                }
                var variable = expression.AsSpan()[varStart..varEnd];
                if (!_globalVariables.TryGetVariable(variable, out var value))
                {
                    throw new RouteConfigurationException($"Variable {variable.ToString()} not defined in {expression}");
                }
                sb.Append(value);
                begin = varEnd + 1; //move to the beginnig of remaining expression
                varStart = expression.IndexOf(_variableStart, begin); //find start of the variable
            }
            while (varStart > 0);
            sb.Append(expression.AsSpan()[begin..expression.Length]);
            return sb.ToString();
        }

        private const string _variableStart = "$(";
        private const char _variableEnd = ')';
    }
}
