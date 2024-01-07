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

public class ConsumerRepositoryTests: RepositoryTestsBase<IConsumersRepository>
{
    protected override ConsumersRepository MakeSut() => new(_fileProvider);

    [Fact]
    public async Task GetConsumer_EmptyMainFields_Exists()
    {
        Guid fileId = Guid.Parse("2437C599-801E-4AFB-AF99-BA9165A9EA53");
        const string consumerName = "Test Consumer";
        const string content = @"
{
    ""Name"":""Test Consumer"",
    ""Subscriptions"":[],
    ""PreRequestActions"":[]
}
";
        SetupReturn(fileId.ToString(), content);

        var result = await _sut.GetConsumerAsync(fileId, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(fileId);
        result!.Name.Should().Be(consumerName);
        result!.Subscriptions.Should().BeEmpty();
        result!.PreRequestActions.Should().Be("[]");
    }

    [Fact]
    public async Task GetConsumer_NonEmptyFields_Exists()
    {
        Guid fileId = Guid.Parse("2437C599-801E-4AFB-AF99-BA9165A9EA54");
        Guid subscriptionId = Guid.Parse("4D81AA95-2887-455D-BE2B-7AE42BC36C2E");
        const string preRequestAction = @"[{""Action"": { ""BlaBla"": ""Bla"" } }]";
        const string consumerName = "Test Consumer";
        const string content = @"
{
    ""Name"":""Test Consumer"",
    ""Subscriptions"":[""4D81AA95-2887-455D-BE2B-7AE42BC36C2E""],
    ""PreRequestActions"":[{""Action"": { ""BlaBla"": ""Bla"" } }]
}
";
        SetupReturn(fileId.ToString(), content);

        var result = await _sut.GetConsumerAsync(fileId, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(fileId);
        result!.Name.Should().Be(consumerName);
        result!.Subscriptions.Should().BeEquivalentTo(new[] { subscriptionId });
        result!.PreRequestActions.Should().Be(preRequestAction);
    }

    [Fact]
    public async Task GetConsumer_NoFile_ReturnsNull()
    {
        Guid fileId = Guid.Parse("2437C599-801E-4AFB-AF99-BA9165A9EA53");
        // No content setup to return from file provider

        var result = await _sut.GetConsumerAsync(fileId, CancellationToken.None);

        result.Should().BeNull();
    }
}

