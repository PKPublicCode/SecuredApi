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
using Microsoft.Extensions.Configuration;
using SecuredApi.Logic.Routing;

namespace SecuredApi.Apps.Gateway.Routing;

public class DefaultGlobalVariablesProvider: IDefaultGlobalVariablesProvider
{
    private readonly IConfigurationSection _config;

    public DefaultGlobalVariablesProvider(IConfigurationSection config)
    {
        _config = config;
    }

    public Task<IEnumerable<KeyValuePair<string, string>>> GetGlobalVariablesAsync(CancellationToken ct)
    {
        return Task.FromResult(
                    _config.AsEnumerable()
                    .   Select(x => new KeyValuePair<string, string>
                                    (
                                        x.Key.Split(":").Last(),
                                        x.Value ?? string.Empty
                                    ))
                    );
    }
}
