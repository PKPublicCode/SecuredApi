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

namespace SecuredApi.Apps.Gateway.Actions;

public static class ActionFactoryBuilderExtensions
{
    public static ActionFactoryBuilder CreateActionFactoryBuilder(this IServiceCollection srv) => new(srv);

    public static IServiceCollection AddActionFactory(this IServiceCollection srv)
    {
        return srv.CreateDefaultActionsFactory()
                .ConfigureActionFactory();
    }

    public static ActionFactoryBuilder CreateDefaultActionsFactory(this IServiceCollection srv)
    {
        return srv.CreateActionFactoryBuilder()
            .AddAction<RemoteCallAction, RemoteCallActionSettings>("RemoteCall")
            .AddAction<SetResponseAction, SetResponseActionSettings>("SetResponse")
            .AddAction<CheckSubscriptionAction, CheckSubscriptionActionSettings>("CheckSubscription")
            .AddAction<SetResponseHeaderAction, SetHeaderActionSettings>("SetResponseHeader")
            .AddAction<SetRequestHeaderAction, SetHeaderActionSettings>("SetRequestHeader")
            .AddScopedAction<RunConsumerActionsAction, EmptySettings>("RunConsumerActions")
            .AddAction<DelayAction, DelayActionSettings>("Delay")
            .AddAction<SetRequestInfoToResponseAction, SetRequestInfoToResponseActionSettings>("SetRequestInfoToResponse")
            .AddAction<SuppressResponseHeadersAction, SuppressHeadersActionSettings>("SuppressResponseHeaders")
            .AddAction<SuppressRequestHeadersAction, SuppressHeadersActionSettings>("SuppressRequestHeaders")
            .AddAction<CheckIPsAction, CheckIPsActionSettings>("CheckIPs")
            .AddAction<ReturnStaticFileAction, ReturnStaticFileActionSettings>("ReturnStaticFile")
            ;
    } 
}
