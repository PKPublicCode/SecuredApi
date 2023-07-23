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
using SecuredApi.Logic.Subscriptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace SecuredApi.Infrastructure.Subscriptions.TableStorage
{
    public class SubscriptionKeysRepository : TableClientBase, ISubscriptionKeysRepository
    {
        public SubscriptionKeysRepository(IOptions<TableClientConfig<ISubscriptionKeysRepository>> config,
                                        ILogger<SubscriptionKeysRepository> logger)
            :base(config.Value, logger)
        {
        }

        private class Entity : TableEntityBase
        {
            public Guid SubscriptionId { get; set; }
            public Guid ConsumerId { get; set; }
            public string? Routes { get; init; }
        }

        public async Task<SubscriptionKeyEntity?> GetSubscriptionKeyAsync(string key, CancellationToken cancellationToken)
        {
            var entity = await GetEntityAsync<Entity>(key, key, cancellationToken);
            if (entity == null)
            {
                return null;
            }

            return new SubscriptionKeyEntity()
            {
                HashedKey = entity.RowKey,
                SubscriptionId = entity.SubscriptionId,
                ConsumerId = entity.ConsumerId,
                Routes = JsonSerializer.Deserialize<Guid[]>(entity.Routes ?? _emptyJsonArray)
                            ?? Array.Empty<Guid>()
            };
        }
    }
}
