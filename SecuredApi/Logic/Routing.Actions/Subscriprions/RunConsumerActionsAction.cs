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
using SecuredApi.Logic.Auth.Subscriptions;
using Microsoft.Extensions.Logging;
using SecuredApi.Logic.Routing.Utils;
using SecuredApi.Logic.Routing.Utils.ResponseStreaming;
using SecuredApi.Logic.Routing.Model.Actions.Auth;

namespace SecuredApi.Logic.Routing.Actions.Subscriptions;

public class RunConsumerActionsAction : IAction
{
    private readonly IConsumersRepository _repo;
    private readonly IOnTheFlyRequestProcessor _processor;
    private readonly ILogger _logger;

    public RunConsumerActionsAction(RunConsumerActions _,
                                    IConsumersRepository repo,
                                    IOnTheFlyRequestProcessor processor,
                                    ILogger<RunConsumerActionsAction> logger)
    {
        _repo = repo;
        _processor = processor;
        _logger = logger;
    }

    public async Task<bool> ExecuteAsync(IRequestContext context)
    {
        if (!context.TryGetConsumerId(out var consumerId))
        {
            _logger.LogError("Subscription data hasn't been loaded. Unable process consumer specific actions");
            return await context.ReturnDataInconsistencyError();
        }

        var consumer = await _repo.GetConsumerAsync(consumerId, context.CancellationToken);
        if (consumer == null)
        {
            //Either inconsistent data, or low probable race condition when user profile was deleted
            _logger.LogError("Unable to load consumer profile {consumerId}", consumerId.ToString());
            return await context.ReturnDataInconsistencyError();
        }

        try
        {
            return await _processor.ProcessRequestAsync(consumer.PreRequestActions, context);
        }
        catch (Exception e)
        {
            //ToDo Consider don't hide and re-throw exception
            _logger.LogError(e, "Error parsing consumer actions");
            return await context.ReturnDataInconsistencyError();
        }
    }
}
