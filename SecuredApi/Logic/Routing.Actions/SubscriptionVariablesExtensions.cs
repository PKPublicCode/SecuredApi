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
using SecuredApi.Logic.Routing.Utils;
using SecuredApi.Logic.Subscriptions;
using System.Diagnostics.CodeAnalysis;

namespace SecuredApi.Logic.Routing.Actions
{
    public static class SubscriptionVariablesExtensions
    {
        public static string Subscription { get; } = "Subscription";
        public static string ConsumerId { get; } = "ConsumerId";
        public static string SubscriptionKeyEntityName { get; } = "SubscriptionKey";


        public static bool TryGetSubscriptionKeyEntity(this IRequestContext ctx, [MaybeNullWhen(false)] out SubscriptionKeyEntity value)
        {
            return ctx.TryGetVariable(SubscriptionKeyEntityName, out value);
        }

        public static void SetSubscriptionKeyEntity(this IRequestContext ctx, SubscriptionKeyEntity e)
        {
            ctx.SetVariable(SubscriptionKeyEntityName, e);
        }
    }
}
