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
using SecuredApi.Logic.FileAccess;
using System.Text;
using SecuredApi.Logic.Common;

namespace SecuredApi.Logic.Subscriptions;

public class ConsumerRepositoryTests
{
    private readonly IFileProvider<IConsumersRepository> _fileProvider;
    private readonly ConsumerRepository _sut;

    public ConsumerRepositoryTests()
    {
        _fileProvider = Substitute.For<IFileProvider<IConsumersRepository>>();
        _sut = new(_fileProvider);
    }

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

        _fileProvider
            .LoadFileAsync(fileId.ToString(), Arg.Any<CancellationToken>())
            .Returns<StreamResult>((x) => throw new FileAccess.FileNotFoundException("Not Found"));

        var result = await _sut.GetConsumerAsync(fileId, CancellationToken.None);

        result.Should().BeNull();
    }

    private void SetupReturn(string fileId, string content)
    {
        _fileProvider
            .LoadFileAsync(fileId, Arg.Any<CancellationToken>())
            .Returns(new StreamResult(new MemoryStream(Encoding.UTF8.GetBytes(content))));
    }
}

