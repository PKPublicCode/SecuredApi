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
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace SecuredApi.ComponentTests.Gateway;

public class DurationTests : GatewayTestsBase
{
    private long? _testRunDuration = null;

    [Fact]
    public async Task Route_Delay()
    {
        Request.SetupGet(RoutePaths.PublicEchoDelay);
        ExpectedResult.StatusCode = StatusCodes.Status200OK;
        ExpectedResult.AddHeaders(Headers.ResponseCommon);
        ExpectedResult.Body = InlineContent.EchoDelay;

        await ExecuteAsync();

        _testRunDuration.Should().BeGreaterThanOrEqualTo(PublicEchoDelayMilliseconds);
    }

    protected override async Task ActAsync()
    {
        var watch = Stopwatch.StartNew();

        await base.ActAsync();

        watch.Stop();
        _testRunDuration = watch.ElapsedMilliseconds;
    }
}

