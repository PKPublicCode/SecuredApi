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

public class ConsumerRepository: RepositoryBase, IConsumersRepository
{
    public ConsumerRepository(IFileProvider<IConsumersRepository> fileProvider)
        : base (fileProvider)
    {
    }

    public async Task<ConsumerEntity?> GetConsumerAsync(Guid id, CancellationToken cancellationToken)
    {
        var e = await GetEntityAsync<Entity>(id.ToString(), cancellationToken);
        if (e != null)
        {
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
        return null;
    }

    // Note: making Entity struct (to reduce load to GC) shouldn't make a big difference,
    // since System.Text.Json anyway operates with type as with object, and so does boxing\unboxing
    protected class Entity
    {
        public string Name { get; init; } = null!;

        public Guid[] Subscriptions { get; init; } = null!;

        [JsonConverter(typeof(ObjectToStringConverter))]
        public string PreRequestActions { get; init; } = null!;
    }
}

