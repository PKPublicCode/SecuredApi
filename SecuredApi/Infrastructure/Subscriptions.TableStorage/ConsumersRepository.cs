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
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SecuredApi.Logic.Subscriptions;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace SecuredApi.Infrastructure.Subscriptions.TableStorage
{
    public class ConsumersRepository: TableClientBase, IConsumersRepository
    {
        public ConsumersRepository(IOptions<TableClientConfig<IConsumersRepository>> config, ILogger<ConsumersRepository> logger)
            :base(config.Value, logger)
        {
        }

        public async Task<ConsumerEntity?> GetConsumerAsync(Guid id, CancellationToken cancellationToken)
        {
            var entity = await GetEntityAsync<Entity>(id.ToString(), id.ToString(), cancellationToken);
            if (entity == null)
            {
                return null;
            }

            return new ConsumerEntity()
            {
                Id = id,
                Subscriptions = JsonSerializer.Deserialize<Guid[]>(entity.Subscriptions)
                            ?? Array.Empty<Guid>(),
                PreRequestActions = entity.PreRequestActions
            };
        }

        private class Entity : TableEntityBase
        {
            public string Subscriptions { get; init; } = null!;
            public string PreRequestActions { get; init; } = null!;
        }
    }
}
