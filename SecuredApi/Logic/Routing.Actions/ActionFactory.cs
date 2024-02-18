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

namespace SecuredApi.Logic.Routing.Actions;

public class ActionFactory : IActionFactory
{
    private readonly Dictionary<string, ActionInfo> _actions;
    private readonly IServiceProvider _srvProvider;

    public ActionFactory(Dictionary<string, ActionInfo> actions, IServiceProvider srvProvider)
    {
        _actions = actions;
        _srvProvider = srvProvider;
    }

    public IAction CreateAction(string name, object settings)
    {
        if (_actions.TryGetValue(name, out var info))
        {
            return (IAction)ActivatorUtilities.CreateInstance(_srvProvider, info.ActionType, settings);
        }
        throw MakeActionNotFoundException(name);
    }

    public Type GetSettingsType(string name)
    {
        if (_actions.TryGetValue(name, out var info))
        {
            return info.SettingsType;
        }
        throw MakeActionNotFoundException(name);
    }

    private static InvalidOperationException MakeActionNotFoundException(string name)
    {
        return new InvalidOperationException($"Action '{name} not found");
    }
}
