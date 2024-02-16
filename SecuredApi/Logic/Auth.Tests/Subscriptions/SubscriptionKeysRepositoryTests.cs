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
namespace SecuredApi.Logic.Auth.Subscriptions;

public class SubscriptionKeysRepositoryTests: RepositoryTestsBase<ISubscriptionKeysRepository>
{
    public SubscriptionKeysRepositoryTests()
    {
    }

    protected override SubscriptionKeysRepository MakeSut() => new(_fileProvider);

    [Fact]
    public async Task GetSubscriptionKey_EmptyNotRequiredFields_Exists()
    {
        const string hash = "2437C599-801E-4AFB-AF99-BA9165A9EA53";
        Guid consumerId = Guid.Parse("2B04A7D7-6257-4577-AE71-1E85645AD65F");
        Guid subscriptionId = Guid.Parse("E81C1972-81B3-437C-8EA2-7620B7652FBC");      
        const string content = @"
        {
            ""ConsumerId"":""2B04A7D7-6257-4577-AE71-1E85645AD65F"",
            ""SubscriptionId"":""E81C1972-81B3-437C-8EA2-7620B7652FBC"",
            ""Routes"":[]
        }";
        SetupReturn(hash, content);
        
        var result = await _sut.GetSubscriptionKeyAsync(hash, CancellationToken.None);

        result.Should().NotBeNull();
        result!.ConsumerId.Should().Be(consumerId);
        result!.SubscriptionId.Should().Be(subscriptionId);
        result!.Routes.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSubscriptionKey_NonEmptyFields_Exists()
    {
        const string hash = "2437C599-801E-4AFB-AF99-BA9165A9EA53";
        Guid consumerId = Guid.Parse("2B04A7D7-6257-4577-AE71-1E85645AD65F");
        Guid subscriptionId = Guid.Parse("E81C1972-81B3-437C-8EA2-7620B7652FBC");
        var routes = new Guid[]
        {
            Guid.Parse("6AFB1BFE-E2D3-4D4A-9236-639B250387EA"),
            Guid.Parse("A479654E-679D-47F0-B1D8-6D6D58379301")
        };
        const string content = @"
{
    ""ConsumerId"":""2B04A7D7-6257-4577-AE71-1E85645AD65F"",
    ""SubscriptionId"":""E81C1972-81B3-437C-8EA2-7620B7652FBC"",
    ""Routes"":[
        ""A479654E-679D-47F0-B1D8-6D6D58379301"",
        ""6AFB1BFE-E2D3-4D4A-9236-639B250387EA""
    ]
}
";
        SetupReturn(hash, content);

        var result = await _sut.GetSubscriptionKeyAsync(hash, CancellationToken.None);

        result.Should().NotBeNull();
        result!.ConsumerId.Should().Be(consumerId);
        result!.SubscriptionId.Should().Be(subscriptionId);
        result!.Routes.Should().BeEquivalentTo(routes);
    }

    [Fact]
    public async Task GetSubscriptionKey_NoFile_ReturnsNull()
    {
        const string hash = "2437C599-801E-4AFB-AF99-BA9165A9EA53";
        // No content setup to return from file provider

        var result = await _sut.GetSubscriptionKeyAsync(hash, CancellationToken.None);

        result.Should().BeNull();
    }
}

