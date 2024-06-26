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

namespace SecuredApi.Apps.Gateway.Configuration;

public static class ConfigurationExtensions
{
    public static T GetRequired<T>(this IConfigurationSection cfg)
        where T: class
    {
        var result = cfg.Get<T>();
        if (result == default)
        {
            throw new ConfigurationException("Not configured: " + cfg.Path);
        }
        return result;
    }
}
