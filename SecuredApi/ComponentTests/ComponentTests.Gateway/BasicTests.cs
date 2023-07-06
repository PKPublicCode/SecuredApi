﻿// Copyright (c) 2021 - present, Pavlo Kruglov.
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
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace SecuredApi.ComponentTests.Gateway;

public class BasicTests: GatewayTestsBase
{
    public BasicTests()
    {
    }


    [Fact]
    public async Task RootRoute_NotFound()
    {
        Request.SetupGet("/");

        await ExecuteAsync();
       
        Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        Response.Headers.Should().Contain(NotFoundResponseHeader);
        Response.Headers.Should().NotContain(CommonResponseHeader);
    }
}

