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
// along with this program. If not, see
// <http://www.mongodb.com/licensing/server-side-public-license>.
using SecuredApi.Logic.Auth.Subscriptions;
using SecuredApi.Logic.Routing.Utils.ResponseStreaming;
using SecuredApi.Logic.Routing.Utils;

namespace SecuredApi.Logic.Routing.Actions.Subscriptions;

public class CheckSubscriptionAction : IAction
{
    private readonly string _subscriptionKeyHeaderName;
    private readonly bool _suppressHeader;
    private readonly StringResponseStream _subscriptionKeyNotSetOrInvalid;
    private readonly StringResponseStream _callNotAllowed;

    public CheckSubscriptionAction(CheckSubscriptionActionSettings settings)
    {
        _subscriptionKeyHeaderName = settings.SubscriptionKeyHeaderName;
        _suppressHeader = settings.SuppressHeader;
        _subscriptionKeyNotSetOrInvalid = new(settings.ErrorNotAuthorizedBody);
        _callNotAllowed = new(settings.ErrorAccessDeniedBody);
    }

    public async Task<bool> ExecuteAsync(IRequestContext context)
    {
        if (!context.Request.Headers.TryGetValue(_subscriptionKeyHeaderName, out var value)
            || value.Count == 0
            || string.IsNullOrEmpty(value[0]))
        {
            return await context.SetNotAuthorizedErrorAsync(_subscriptionKeyNotSetOrInvalid);
        }

        var hash = context.GetRequiredService<IHashCalculator>()
                            .CalculateHash(value[0]!);

        var subscriptionKey = await context.GetRequiredService<ISubscriptionKeysRepository>()
                                        .GetSubscriptionKeyAsync(hash, context.CancellationToken);
        if (subscriptionKey == null)
        {
            return await context.SetNotAuthorizedErrorAsync(_subscriptionKeyNotSetOrInvalid);
        }

        if (!context.Route.GroupIds.Overlaps(subscriptionKey.Routes))
        {
            return await context.SetAccessDeniedErrorAsync(_callNotAllowed);
        }

        if(_suppressHeader)
        {
            context.Request.Headers.Remove(_subscriptionKeyHeaderName);
        }

        //For consideration. Here we store complex object (object + array) into dictionary.
        //Technically, Guid ConsumerId only is enougth, But if we save Guid into dictionary of objects,
        //it will cause boxing. So we release big object, but allocate smaller one.
        context.SetConsumerId(subscriptionKey.ConsumerId);
        return true;
    }
}
