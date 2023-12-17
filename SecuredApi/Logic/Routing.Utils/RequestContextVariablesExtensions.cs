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
using System.Diagnostics.CodeAnalysis;
namespace SecuredApi.Logic.Routing.Utils;

public static class RequestContextVariablesExtensions
{
    public static void SetVariable(this IRequestContext ctx, string name, object o)
    {
        ctx.Variables.SetVariable(name, o);
    }

    public static T GetVariable<T>(this IRequestContext context, string name)
    {
        return (T) context.Variables.GetVariable(name);
    }

    public static bool TryGetVariable<T>(this IRequestContext context, string name, [MaybeNullWhen(false)] out T value)
    {
        if (context.Variables.TryGetVariable(name, out var o)
            && o is T t)
        {
            value = t;
            return true;
        }
        value = default;
        return false;
    }
}
