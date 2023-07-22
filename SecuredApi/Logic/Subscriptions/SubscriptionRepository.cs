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

namespace SecuredApi.Logic.Subscriptions;

public class SubscriptionRepository: RepositoryBase, ISubscriptionsRepository
{
    public SubscriptionRepository(IFileProvider<ISubscriptionsRepository> fileProvider)
        : base(fileProvider)
    {
    }

    public async Task<SubscriptionEntity?> GetSubscriptionAsync(Guid id, Guid consumerId, CancellationToken cancellationToken)
    {
        var e = await GetEntityAsync<Entity>(id.ToString(), cancellationToken);
        if (e != null)
        {
            if (e.ConsumerId == Guid.Empty)
            {
                throw new DataCorruptedException("Subscriptions.ConsumerId can't be null or empty");
            }

            return new()
            {
                Id = id,
                Routes = e.Routes,
                HashedKeys = e.HashedKeys
            };
        }
        return null;
    }

    private class Entity
    {
        public Guid ConsumerId { get; set; }
        public Guid[] Routes { get; set; } = Array.Empty<Guid>();
        public Guid[] HashedKeys { get; set; } = Array.Empty<Guid>();
    }
}

