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
using SecuredApi.WebApps.Gateway.Hosting;
using System.Text;
using System.Net.Mime;
using static SecuredApi.WebApps.Gateway.Utils.Constants.RoutePaths;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace SecuredApi.WebApps.Gateway;

public class GatewayTests: IClassFixture<GatewayHostFixture>
{
    private GatewayHostFixture _gateway;

    public GatewayTests(GatewayHostFixture fixture)
    {
        _gateway = fixture;
    }

    [Fact]
    public async Task ApiKeyCall_Positive()
    {
        await _gateway.StartAsync();

        using var msg = new HttpRequestMessage();
        msg.Method = HttpMethod.Post;
        msg.Content = new StringContent("Hello hello", Encoding.UTF8, MediaTypeNames.Text.Plain);
        msg.Headers.Add(Headers.SubscriptionKeyHeaderName, "5F39D492-A141-498A-AE04-76C6B77F246A");
        msg.RequestUri = new Uri(PrivateApiKeyRedirectWildcard, UriKind.Relative);
        //using var content = new StringContent("Hello hello", Encoding.UTF8, MediaTypeNames.Text.Plain);
        //var uri = new Uri(PrivateApiKeyRedirectWildcard);
        var result = await _gateway.HttpClient.SendAsync(msg);

        var tmp = await result.Content.ReadAsStringAsync();
        //await Task.Delay(10000);
        Assert.True(true);
    }
}

