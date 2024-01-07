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

namespace SecuredApi.Logic.Auth.Subscriptions;

public class SubscriptionKeysRepository: RepositoryBase, ISubscriptionKeysRepository
{
    public SubscriptionKeysRepository(IFileProvider<ISubscriptionKeysRepository> fileProvider)
        :base(fileProvider)
    {
    }

    public async Task<SubscriptionKeyEntity?> GetSubscriptionKeyAsync(string hash, CancellationToken ct)
    {
        var e = await GetEntityAsync<Entity>(hash, ct);

        if (e != null)
        {
            if (e.ConsumerId == Guid.Empty
                || e.SubscriptionId == Guid.Empty)
            {
                throw new DataCorruptedException("SubscriptionKeys.ConsumerId or SubscriptionId can't be null or empty");
            }

            return new()
            {
                HashedKey = hash,
                ConsumerId = e.ConsumerId,
                SubscriptionId = e.SubscriptionId,
                Routes = e.Routes
            };
        }
        return null;
    }

    private record Entity
    (
        Guid ConsumerId,
        Guid SubscriptionId,
        Guid[] Routes
    );
}

