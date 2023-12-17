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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SecuredApi.Logic.Routing.Utils;

public static class RequestContextDiExtensions
{
    public static T GetRequiredService<T>(this IRequestContext c) => c.ServiceProvider.GetRequiredService<T>();
    public static T GetService<T>(this IRequestContext c) => c.ServiceProvider.GetService<T>();
    public static T CreateInstance<T>(this IRequestContext c) => ActivatorUtilities.CreateInstance<T>(c.ServiceProvider);
    public static ILogger<T> GetLogger<T>(this IRequestContext c) => c.ServiceProvider.GetRequiredService<ILogger<T>>();
}
