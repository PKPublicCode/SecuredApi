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
using Microsoft.Extensions.DependencyInjection;
using Base = SecuredApi.Infrastructure.Configuration;
using SecuredApi.Logic.FileAccess;
using SecuredApi.Infrastructure.FileAccess.AzureStorage;

namespace SecuredApi.Infrastructure.AzureConfiguration;

public class AzureFileAccessConfigurator : Base.FileAccessConfigurator
{
    public override Action<IServiceCollection, IConfiguration>? GetConfigurator<TClient>(string name)
        => name switch
        {
            "AzureStorage" => (srv, cfg) => srv.AddSingleton<IFileProvider<TClient>, FileProvider<TClient>>()
                                            .Configure<FileProviderConfig<TClient>>(cfg),
            _ => base.GetConfigurator<TClient>(name)
        };
}

