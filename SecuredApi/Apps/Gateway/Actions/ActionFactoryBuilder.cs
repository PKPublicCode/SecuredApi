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
using Microsoft.Extensions.DependencyInjection;
using SecuredApi.Logic.Routing;
using System.Collections.Generic;
using SecuredApi.Logic.Routing.Actions;
using SecuredApi.Apps.Gateway.Configuration;

namespace SecuredApi.Apps.Gateway.Actions
{
    public class ActionFactoryBuilder
    {
        private readonly IServiceCollection _services;
        private readonly Dictionary<string, ActionInfo> _actions;

        public ActionFactoryBuilder(IServiceCollection srv)
        {
            _services = srv;
            _actions = new();
        }

        public ActionFactoryBuilder AddAction<TAction, TSettings>(string name)
            where TAction : IAction
        {
            _actions[name] = MakeAction<TAction, TSettings>();
            return this;
        }

        public ActionFactoryBuilder AddScopedAction<TAction, TSettings>(string name)
            where TAction : class, IScopedAction<TSettings>
        {
            _actions[name] = MakeScopedAction<TAction, TSettings>();
            return this;
        }

        public IServiceCollection ConfigureActionFactory()
        {
            var factory = new ActionFactory(_actions);
            _services.AddSingleton<IActionFactory>(factory);
            return _services;
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
                ActionCtor: action.GetConstructor(new[] { settings })
                                    ?? throw new ConfigurationException($"Action {action.Name} doesn't have valid constructor"),
                SettingsType: settings
            );
        }
    }
}
