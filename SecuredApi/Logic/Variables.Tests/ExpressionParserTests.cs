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
using Xunit;
using System.Diagnostics.CodeAnalysis;

namespace SecuredApi.Logic.Variables;

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
    public void Parse_ProperlyFormedExpression_Success(string expression, string expected)
    {
        var sut = new GlobalExpressionProcessor(new SimplePositiveGlobalVariables());

        var result = sut.Parse(expression);

        Assert.Equal(expected, result);
    }

    [Fact()]
    public void Parse_MalformedExpression_InvalidExpressionExpression()
    {
        var sut = new GlobalExpressionProcessor(new SimplePositiveGlobalVariables());

        Assert.Throws<InvalidExpressionException>(() => sut.Parse("blabla$(asfasdf"));
    }

    [Fact()]
    public void Parse_VariableNotExist_InvalidExpressionException()
    {
        var sut = new GlobalExpressionProcessor(new NotFoundeGlobalVariables());

        Assert.Throws<InvalidExpressionException>(() => sut.Parse("Hello$(value)"));
    }

    //Unable to mock methods with ReadOnlySpan with NSubstitute. Need more research
    private class SimplePositiveGlobalVariables : IGlobalVariables
    {
        public string GetVariable(string key)
        {
            throw new NotImplementedException();
        }

        public string GetVariable(ReadOnlySpan<char> key)
        {
            throw new NotImplementedException();
        }

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
        public string GetVariable(string key)
        {
            throw new NotImplementedException();
        }

        public string GetVariable(ReadOnlySpan<char> key)
        {
            throw new NotImplementedException();
        }

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
