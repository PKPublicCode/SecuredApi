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
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace SecuredApi.Infrastructure.Subscriptions.TableStorage
{
    public class SubscriptionsRepository : TableClientBase, ISubscriptionsRepository
    {
        public SubscriptionsRepository(IOptions<TableClientConfig<ISubscriptionsRepository>> config,
                                        ILogger<SubscriptionKeysRepository> logger)
            : base(config.Value, logger)
        {
        }

        public async Task<SubscriptionEntity?> GetSubscriptionAsync(Guid id, Guid consumerId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Under refactoring");
            var entity = await GetEntityAsync<Entity>(consumerId.ToString(), id.ToString(), cancellationToken);
            if (entity == null)
            {
                return null;
            }

            return new SubscriptionEntity()
            {
                Id = id,
                ConsumerId = entity.ConsumerId,
                Routes = JsonSerializer.Deserialize<Guid[]>(entity.Routes ?? _emptyJsonArray)
                            ?? Array.Empty<Guid>() //,
                //HashedKeys = JsonSerializer.Deserialize<Guid[]>(entity.SubscriptionKeys ?? _emptyJsonArray)
                //            ?? Array.Empty<Guid>()
            };
        }

        private class Entity: TableEntityBase
        {
            public Guid ConsumerId { get; init; }
            public string? Routes { get; init; }
            public string? SubscriptionKeys { get; init; }
        }
    }
}
