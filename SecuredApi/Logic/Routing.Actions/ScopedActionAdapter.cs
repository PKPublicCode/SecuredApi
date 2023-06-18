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
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace SecuredApi.Logic.Routing.Actions
{
    public class ScopedActionAdapter<TAction, TActionSettings> : IAction
        where TAction : class, IScopedAction<TActionSettings>
    {
        private readonly TActionSettings _settings;

        public ScopedActionAdapter(TActionSettings settings)
        {
            _settings = settings;
        }

        public Task<bool> ExecuteAsync(IRequestContext context)
        {
            var action = context.ServiceProvider.GetRequiredService<TAction>();
            return action.ExecuteAsync(context, _settings);
        }
    }
}
