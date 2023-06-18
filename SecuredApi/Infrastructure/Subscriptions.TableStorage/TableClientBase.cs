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
using System.Threading.Tasks;
using Azure.Data.Tables;
using Azure;
using Microsoft.Extensions.Logging;
using System.Threading;
using SecuredApi.Logic.Subscriptions;
using Azure.Identity;

namespace SecuredApi.Infrastructure.Subscriptions.TableStorage
{
    public abstract class TableClientBase
    {
        protected const string _emptyJsonArray = "[]";

        protected readonly TableClient _client;
        protected readonly ILogger _logger;

        protected TableClientBase(TableClientConfig config,
                               ILogger logger)
        {
            _logger = logger;
            if (config.Rbac != null)
            {
                _client = CreateClient(config.Rbac);
            }
            else if (config.SharedKey != null)
            {
                _client = CreateClient(config.SharedKey);
            }
            else
            {
                throw new InvalidOperationException("Table client configuration not specified");
            }
        }

        private static TableClient CreateClient(RbacConfig cfg) => new
                (
                    new Uri(cfg.Endpoint),
                    cfg.TableName,
                    new DefaultAzureCredential()
                );

        private static TableClient CreateClient(SharedKeyConfig cfg) => new
                (
                    new Uri(cfg.Endpoint),
                    cfg.TableName,
                    new TableSharedKeyCredential(cfg.AccountName, cfg.StorageAccountKey)
                );

        protected async Task<Entity?> GetEntityAsync<Entity>(string partitionKey, string rowKey, CancellationToken cancellationToken)
            where Entity : class, ITableEntity, new()
        {
            Response<Entity> response;
            try
            {
                response = await _client.GetEntityAsync<Entity>(partitionKey, rowKey, cancellationToken: cancellationToken);
            }
            catch (RequestFailedException e) when (e.Status == 404)
            {
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError("Unexpected exception calling Table Storage", e);
                throw new DataStorageConnectionErrorException("Unable retrieve data from storage", e);
            }
            return response.Value;
        }
    }
}
