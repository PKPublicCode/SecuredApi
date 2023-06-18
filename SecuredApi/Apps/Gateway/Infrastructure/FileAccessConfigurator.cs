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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using SecuredApi.Logic.FileAccess;
using FS = SecuredApi.Infrastructure.FileAccess.FileSystem;
using AS = SecuredApi.Infrastructure.FileAccess.AzureStorage;

namespace SecuredApi.Apps.Gateway.Infrastructure
{
    public class FileAccessConfigurator : IInfrastructureConfigurator
    {
        public string SectionName => "FileAccess";

        public virtual Action<IServiceCollection, IConfiguration>? GetConfigurator<TClient>(string name)
            => name switch
            {
                "AzureStorage" => (srv, cfg) => srv.AddSingleton<IFileProvider<TClient>, AS.FileProvider<TClient>>()
                                                .Configure<AS.FileProviderConfig<TClient>>(cfg),
                "FileSystem" => (srv, cfg) => srv.AddSingleton<IFileProvider<TClient>, FS.FileProvider<TClient>>(),
                _ => null
            };
    }
}
