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
using System.Linq;
using System.Threading.Tasks;
using SecuredApi.Logic.Subscriptions;
using SecuredApi.Logic.Routing.Utils.ResponseStreaming;
using SecuredApi.Logic.Routing.Utils;

namespace SecuredApi.Logic.Routing.Actions.Subscriptions
{
    public class CheckSubscriptionAction : IAction
    {
        private readonly string _subscriptionKeyHeaderName;
        private readonly bool _suppressHeader;

        public CheckSubscriptionAction(CheckSubscriptionActionSettings settings)
        {
            _subscriptionKeyHeaderName = settings.SubscriptionKeyHeaderName;
            _suppressHeader = settings.SuppressHeader;
        }

        public async Task<bool> ExecuteAsync(IRequestContext context)
        {
            if (!context.Request.Headers.TryGetValue(_subscriptionKeyHeaderName, out var value)
                || value.Count == 0
                || string.IsNullOrEmpty(value[0]))
            {
                return await context.SetAccessDeniedErrorAsync(_subscriptionKeyHeaderNotSetError);
            }

            var subscriptionKey = await context.GetRequiredService<ISubscriptionKeysRepository>()
                                            .GetSubscriptionKeyAsync(value[0]!, context.CancellationToken);
            if (subscriptionKey == null)
            {
                return await context.SetAccessDeniedErrorAsync(_subscriptionKeyNotFoundError);
            }

            if (!CheckSubscription(subscriptionKey, context))
            {
                return await context.SetAccessDeniedErrorAsync(_callNotAllowedForYourSubscriptionError);
            }

            if(_suppressHeader)
            {
                context.Request.Headers.Remove(_subscriptionKeyHeaderName);
            }

            //For consideration. Here we store complex object (object + array) into dictionary.
            //Technically, Guid ConsumerId only is enougth, But if we save Guid into dictionary of objects,
            //it will cause boxing. So we release big object, but allocate smaller one.
            context.SetSubscriptionKeyEntity(subscriptionKey);
            return true;
        }

        private static bool CheckSubscription(SubscriptionKeyEntity subscription, IRequestContext context)
        {
            var allowed = subscription.Routes.ToHashSet();
            for (int i = 0; i < context.RoutesGroups.Count; ++i)
            {
                if(allowed.Contains(context.RoutesGroups[i].Id))
                {
                    return true;
                }
            }
            return false;
        }

        private static readonly StringResponseStream _subscriptionKeyHeaderNotSetError = new("Subscription key header is not set");
        private static readonly StringResponseStream _subscriptionKeyNotFoundError = new("Subscription key not found");
        private static readonly StringResponseStream _callNotAllowedForYourSubscriptionError = new("Call not allowed for your subscription");
    }
}
