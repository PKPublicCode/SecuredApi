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
using System.Net;
using static SecuredApi.WebApps.Gateway.Utils.Constants.RoutePaths;

namespace SecuredApi.WebApps.Gateway;

[Collection("Gateway runner")]
public class StaticContentGatewayTests: TestsBase
{
    public StaticContentGatewayTests(GatewayHostFixture fixture)
        : base(fixture)
    {
    }

    [Theory]
    [InlineData(PublicContent.WelcomeHtml.Path, PublicContent.WelcomeHtml.Content)]
    [InlineData(PublicContent.WildcardHelloTxt.Path, PublicContent.WildcardHelloTxt.Content)]
    public async Task WildcardPath_ExistingContent_StatusOk(string path, string expectedContent)
    {
        Request.SetGet()
            .SetRelativePath($"{PublicStaticContent}{path}");

        ExpectedResult.Body = expectedContent;
        ExpectedResult.StatusCode = HttpStatusCode.OK;

        await ActAsync();
        await AssertAsync();
    }

    [Fact]
    public async Task WildcardPath_NotExistingContent_NotFound()
    {
        Request.SetGet()
            .SetRelativePath($"{PublicStaticContent}/blablabla.html");

        ExpectedResult.Body = InlineContent.StaticFileNotFound;
        ExpectedResult.StatusCode = HttpStatusCode.NotFound;
        ExpectedResult.AddHeaders(Headers.ResponseCommonOnError);

        await ActAsync();
        await AssertAsync();
    }
}

