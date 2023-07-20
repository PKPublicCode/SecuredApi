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
namespace SecuredApi.Logic.Routing.Actions;

public class ActionFactory : IActionFactory
{
    private readonly Dictionary<string, ActionInfo> _actions;

    public ActionFactory(Dictionary<string, ActionInfo> actions)
    {
        _actions = actions;
    }

    public IAction CreateAction(string name, object settings)
    {
        if (_actions.TryGetValue(name, out var info))
        {
            return (IAction)info.ActionCtor.Invoke(new[] { settings });
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
