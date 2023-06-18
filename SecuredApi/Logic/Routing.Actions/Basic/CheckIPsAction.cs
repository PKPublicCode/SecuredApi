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
using System.Collections.Generic;
using System.Threading.Tasks;
using SecuredApi.Logic.Routing.Utils.ResponseStreaming;

namespace SecuredApi.Logic.Routing.Actions.Basic
{
    public class CheckIPsAction : IAction
    {
        private HashSet<string> _whiteList;
        public CheckIPsAction(CheckIPsActionSettings settings)
        {
            _whiteList = settings.WhiteList;
        }

        public Task<bool> ExecuteAsync(IRequestContext context)
        {
            var current = context.ConnectionInfo.RemoteIpAddress.ToString();
            if(_whiteList.Contains(current))
            {
                return Task.FromResult(true);
            }
            return context.SetAccessDeniedErrorAsync(_callNotAllowed);
        }
        private static readonly StringResponseStream _callNotAllowed = new("IP not allowed");
    }
}
