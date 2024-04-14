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
using SecuredApi.Logic.Routing.Utils.ResponseStreaming;
using SecuredApi.Logic.Routing.Model.Actions.Basic;

namespace SecuredApi.Logic.Routing.Actions.Basic;

public class CheckIPsAction : IAction
{
    private readonly HashSet<string> _whiteList;
    private readonly int _noAccessStatusCode;
    private readonly string _noAccessResponseBody;

    public CheckIPsAction(CheckIPs settings)
    {
        _whiteList = settings.WhiteList;
        _noAccessStatusCode = settings.NoAccessStatusCode;
        _noAccessResponseBody = settings.NoAccessResponseBody;
    }

    public Task<bool> ExecuteAsync(IRequestContext context)
    {
        var current = context.ConnectionInfo.RemoteIpAddress?.ToString()
            ?? string.Empty;
        if(_whiteList.Contains(current))
        {
            return Task.FromResult(true);
        }
        return context.SetResponseAsync(_noAccessStatusCode, _noAccessResponseBody);
    }
}
