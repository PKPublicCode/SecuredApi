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
using SecuredApi.Logic.Common;
using System.Text.Json.Serialization;

namespace SecuredApi.Logic.Subscriptions;

public class ConsumerRepository: IConsumersRepository
{
    private readonly IFileProvider _fileProvider;

    public ConsumerRepository(IFileProvider<IConsumersRepository> fileProvider)
    {
        _fileProvider = fileProvider;
    }

    public async Task<ConsumerEntity?> GetConsumerAsync(Guid id, CancellationToken cancellationToken)
    {
        StreamResult c;
        try
        {
            c = await _fileProvider.LoadFileAsync(id.ToString(), cancellationToken);
        }
        catch(FileAccess.FileNotFoundException)
        {
            return null;
        }

        using (c)
        {
            var e = c.Content.DeserializeJson<Entity>();

            if (e.Subscriptions == null)
            {
                throw new DataCorruptedException("Consumer.Subscriptions can't be null");
            }

            if (e.PreRequestActions.IsNullOrEmpty())
            {
                throw new DataCorruptedException("Consumer.PreRequestActions can't be null or empty");
            }

            return new()
            {
                Id = id,
                Name = e.Name,
                Subscriptions = e.Subscriptions,
                PreRequestActions = e.PreRequestActions
            };
        }
    }

    private class Entity
    {
        public string Name { get; init; } = null!;

        public Guid[] Subscriptions { get; init; } = null!;

        [JsonConverter(typeof(ObjectToStringConverter))]
        public string PreRequestActions { get; init; } = null!;
    }
}

