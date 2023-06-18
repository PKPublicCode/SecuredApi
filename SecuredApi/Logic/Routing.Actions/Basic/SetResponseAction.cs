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
using System.Threading.Tasks;
using SecuredApi.Logic.Routing.Utils.ResponseStreaming;

namespace SecuredApi.Logic.Routing.Actions.Basic
{
    public class SetResponseAction : IAction
    {
        private readonly int _code;
        private readonly StringResponseStream _body;

        public SetResponseAction(SetResponseActionSettings settings)
        {
            _code = settings.HttpCode;
            //Dispite StringResponseStream is IDisposable, for the sake of efficiancy making it a singleton.
            //StringResponseStream.Dispose does nothing
            _body = settings.Body ?? StringResponseStream.Empty;
        }

        public Task<bool> ExecuteAsync(IRequestContext context)
        {
            context.Response.StatusCode = _code;
            context.Response.Body = _body;
            return Task.FromResult(true);
        }
    }
}
