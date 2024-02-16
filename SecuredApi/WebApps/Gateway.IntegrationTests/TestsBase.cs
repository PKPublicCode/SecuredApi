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
using SecuredApi.WebApps.Gateway.Fixtures;

namespace SecuredApi.WebApps.Gateway;

public abstract class TestsBase: IDisposable
{
    protected readonly ExpectedResult ExpectedResult = new();
    protected readonly HttpRequestMessage Request = new();
    protected readonly GatewayHostFixture Gateway;
    protected HttpResponseMessage? Response;

    public TestsBase(GatewayHostFixture gateway)
    {
        Gateway = gateway;
    }

    public void Dispose()
    {
        Request.Dispose();
    }

    protected virtual async Task ActAsync()
    {
        await Gateway.StartAsync();
        Response = await Gateway.HttpClient.SendAsync(Request);
    }

    protected virtual async Task AssertAsync()
    {
        if (Response == null)
        {
            throw new InvalidOperationException("Server not called");
        }

        Response.StatusCode.Should().Be(ExpectedResult.StatusCode);

        // skip headers that out our control
        Response.Headers.Where(x => !_serverHeaders.Contains(x.Key))
            .Should().BeEquivalentTo(ExpectedResult.Headers);
        (await Response.Content.ReadAsStringAsync())
            .Should().BeEquivalentTo(ExpectedResult.Body);
    }

    private static readonly string[] _serverHeaders = {
        "Transfer-Encoding",
        "Date",
        "Server"
    };
}

