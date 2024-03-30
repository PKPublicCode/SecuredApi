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
using SecuredApi.Logic.Routing;
using SecuredApi.Logic.Routing.Actions;
using SecuredApi.Apps.Gateway.Configuration;

namespace SecuredApi.Apps.Gateway.Actions;

public class ActionsBuilder
{
    private readonly IServiceCollection _services;
    private readonly Dictionary<string, ActionInfo> _actions;

    public ActionsBuilder(IServiceCollection srv)
    {
        _services = srv;
        _actions = new();
    }

    public ActionsBuilder AddAction<TAction, TSettings>(string name)
        where TAction : IAction
    {
        _actions[name] = MakeAction<TAction, TSettings>();
        return this;
    }

    public ActionsBuilder AddScopedAction<TAction, TSettings>(string name)
        where TAction : class, IScopedAction<TSettings>
    {
        _actions[name] = MakeScopedAction<TAction, TSettings>();
        return this;
    }

    public IServiceCollection ConfigureActionFactory()
    {
        return _services.AddSingleton(_actions);
    }

    private static ActionInfo MakeAction<TAction, TSettings>()
    where TAction : IAction
    {
        return MakeAction(typeof(TAction), typeof(TSettings));
    }

    private static ActionInfo MakeScopedAction<TAction, TSettings>()
        where TAction : class, IScopedAction<TSettings>
    {
        return MakeAction(typeof(ScopedActionAdapter<TAction, TSettings>), typeof(TSettings));
    }

    private static ActionInfo MakeAction(Type action, Type settings)
    {
        return new ActionInfo
        (
            ActionType: action,
            SettingsType: settings
        );
    }
}
