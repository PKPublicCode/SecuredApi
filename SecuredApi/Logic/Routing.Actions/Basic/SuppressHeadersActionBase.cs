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
using Microsoft.AspNetCore.Http;
using SecuredApi.Logic.Routing.Model.Actions.Basic;

namespace SecuredApi.Logic.Routing.Actions.Basic;

public abstract class SuppressHeadersActionBase : IAction
{
    private readonly List<string> _headers;

    public SuppressHeadersActionBase(ISuppressHeader settings)
    {
        _headers = settings.Headers;
    }

    public Task<bool> ExecuteAsync(IRequestContext context)
    {
        var contextHeaders = GetContextHeaders(context);
        foreach(var h in _headers)
        {
            contextHeaders.Remove(h);
        }
        return Task.FromResult(true);
    }

    protected abstract IHeaderDictionary GetContextHeaders(IRequestContext context);
}
