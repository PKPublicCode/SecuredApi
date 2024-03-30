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
using SecuredApi.Logic.Common;
using SecuredApi.Logic.FileAccess;

namespace SecuredApi.Logic.Auth.Subscriptions;

public abstract class RepositoryBase
{
    private readonly IFileProvider _fileProvider;

    protected RepositoryBase(IFileProvider fileProvider)
    {
        _fileProvider = fileProvider;
    }

    protected async Task<T?> GetEntityAsync<T>(string fileId, CancellationToken ct)
        where T: class
    {
        StreamResult content;
        try
        {
            content = await _fileProvider.LoadFileAsync(fileId, ct);
        }
        catch (FileAccess.FileNotFoundException)
        {
            return null;
        }

        using (content)
        {
            return content.Content.DeserializeJson<T>();
        }
    }
}

