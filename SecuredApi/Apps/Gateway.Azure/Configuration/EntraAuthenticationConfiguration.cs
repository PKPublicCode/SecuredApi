﻿// Copyright (c) 2021 - present, Pavlo Kruglov.
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
using Microsoft.Extensions.DependencyInjection;
using SecuredApi.Logic.Auth.Jwt;

namespace SecuredApi.Apps.Gateway.Azure.Configuration;

public static class EntraAuthenticationConfiguration
{
    public static IServiceCollection ConfigureEntraAuthentication(this IServiceCollection srv)
    {
        return srv.AddSingleton<ISigningKeysProvider, SigningKeysProvider>();
    }
}

