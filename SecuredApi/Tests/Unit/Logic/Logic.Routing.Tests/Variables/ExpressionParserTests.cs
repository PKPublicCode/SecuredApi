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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NSubstitute;
using System.Diagnostics.CodeAnalysis;

namespace SecuredApi.Logic.Routing.Variables
{
    public class ExpressionParserTests
    {
        public ExpressionParserTests()
        {
        }

        [Theory()]
        [InlineData("", "")]
        [InlineData("Hello", "Hello")]
        [InlineData("Hello$(value)", "Hello_value_")]
        [InlineData("$(value)", "_value_")]
        [InlineData("$(value)Hello", "_value_Hello")]
        [InlineData("Hello$(value)BlaBla", "Hello_value_BlaBla")]
        [InlineData("Hello$(value)BlaBla$(value1)blu$(value3)ble", "Hello_value_BlaBla_value1_blu_value3_ble")]
        public void Positive(string expression, string expected)
        {
            var sut = new ExpressionProcessor(new SimplePositiveGlobalVariables());
            Assert.Equal(expected, sut.ConvertExpression(expression));
        }

        [Fact()]
        public void InvalidExpression()
        {
            var sut = new ExpressionProcessor(new SimplePositiveGlobalVariables());
            Assert.Throws<RouteConfigurationException>(() => sut.ConvertExpression("blabla$(asfasdf"));
        }

        [Fact()]
        public void NotFoundVariable()
        {
            var sut = new ExpressionProcessor(new NotFoundeGlobalVariables());
            Assert.Throws<RouteConfigurationException>(() => sut.ConvertExpression("Hello$(value)"));
        }

        //Unable to mock methods with ReadOnlySpan with NSubstitute. Need more research
        private class SimplePositiveGlobalVariables : IGlobalVariables
        {
            public bool TryGetVariable(string key, [MaybeNullWhen(false)] out string value)
            {
                value = $"_{key}_";
                return true;
            }

            public bool TryGetVariable(ReadOnlySpan<char> key, [MaybeNullWhen(false)] out string value)
            {
                return TryGetVariable(key.ToString(), out value);
            }
        }

        private class NotFoundeGlobalVariables : IGlobalVariables
        {
            public bool TryGetVariable(string key, [MaybeNullWhen(false)] out string value)
            {
                value = null;
                return false;
            }

            public bool TryGetVariable(ReadOnlySpan<char> key, [MaybeNullWhen(false)] out string value)
            {
                return TryGetVariable(key.ToString(), out value);
            }
        }
    }
}
