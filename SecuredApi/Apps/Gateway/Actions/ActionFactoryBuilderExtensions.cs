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
using SecuredApi.Logic.Routing.Actions.Basic;
using SecuredApi.Logic.Routing.Actions.Subscriptions;
using SecuredApi.Logic.Routing.Actions.OAuth;
using SecuredApi.Logic.Routing.Model.Actions.Basic;
using SecuredApi.Logic.Routing.Model.Actions.Auth;
using SecuredApi.Logic.Routing;
using SecuredApi.Logic.Routing.Actions;

namespace SecuredApi.Apps.Gateway.Actions;

public static class ActionFactoryBuilderExtensions
{
    public static IServiceCollection AddActionFactory(this IServiceCollection srv)
    {
        return srv.CreateDefaultActionsFactory()
                .ConfigureActionFactory()
                .AddSingleton<IActionFactory, ActionFactory>(); ;
    }

    private static ActionsBuilder CreateActionFactoryBuilder(this IServiceCollection srv) => new(srv);

    private static ActionsBuilder CreateDefaultActionsFactory(this IServiceCollection srv)
    {
        return srv.CreateActionFactoryBuilder()
            .AddAction<RemoteCallAction, RemoteCall>()
            .AddAction<SetResponseAction, SetResponse>()
            .AddAction<CheckSubscriptionAction, CheckSubscription>()
            .AddAction<SetResponseHeaderAction, SetResponseHeader>()
            .AddAction<SetRequestHeaderAction, SetRequestHeader>()
            .AddAction<RunConsumerActionsAction, RunConsumerActions>()
            .AddAction<DelayAction, Delay>()
            .AddAction<SetRequestInfoToResponseAction, SetRequestInfoToResponse>()
            .AddAction<SuppressResponseHeadersAction, SuppressResponseHeaders>()
            .AddAction<SuppressRequestHeadersAction, SuppressRequestHeaders>()
            .AddAction<CheckIPsAction, CheckIPs>()
            .AddAction<ReturnStaticFileAction, ReturnStaticFile>()
            .AddAction<CheckEntraJwtAction, CheckEntraJwt>()
            .AddAction<CheckEntraJwtClaimsAction, CheckEntraJwtClaims>()
            ;
    } 
}
