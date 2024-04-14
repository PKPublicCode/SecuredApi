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
using SecuredApi.Logic.Routing.Model.RuntimeVariables;

namespace SecuredApi.Logic.Routing.Utils;

public static class RequestContextCommonVariablesExtensions
{
    public static void SetRequestRemainingPath(this IRequestContext ctx, string value)
    {
        ctx.Variables.SetVariable(Request.RemainingPath, value);
    }

    public static void SetRequestHttpMethod(this IRequestContext ctx, string value)
    {
        ctx.Variables.SetVariable(Request.HttpMethod, value);
    }

    public static void SetRequestQueryString(this IRequestContext ctx, string value)
    {
        ctx.Variables.SetVariable(Request.QueryString, value);
    }

    public static void SetConsumerId(this IRequestContext ctx, Guid value)
    {
        // Warning. Guid will be boxed!
        ctx.SetVariable(Auth.ConsumerId, value);
    }

    public static bool TryGetConsumerId(this IRequestContext ctx, [MaybeNullWhen(false)] out Guid value)
    {
        return ctx.TryGetVariable(Auth.ConsumerId, out value);
    }
}

