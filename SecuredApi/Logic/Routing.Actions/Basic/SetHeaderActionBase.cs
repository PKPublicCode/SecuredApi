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

namespace SecuredApi.Logic.Routing.Actions.Basic;

public abstract class SetHeaderActionBase : IAction
{
    private readonly string _key;
    private readonly string _value;

    protected SetHeaderActionBase(SetHeaderActionSettings settings)
    {
        _key = settings.Key;
        _value = settings.Value;
    }

    protected abstract IHeaderDictionary GetHeaders(IRequestContext context);

    public Task<bool> ExecuteAsync(IRequestContext context)
    {
        GetHeaders(context).Append(_key, _value);
        return Task.FromResult(true);
    }
}
