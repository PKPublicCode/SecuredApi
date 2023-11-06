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

namespace SecuredApi.Logic.Routing.Actions.Basic;

public class SetRequestInfoToResponseAction : IAction
{
    private readonly string _headLine;
    private readonly int _httpCode;

    public SetRequestInfoToResponseAction(SetRequestInfoToResponseActionSettings settings)
    {
        _headLine = settings.HeadLine;
        _httpCode = settings.HttpCode;
    }

    public Task<bool> ExecuteAsync(IRequestContext context)
    {
        var response = context.Response;
        var request = context.Request;
        response.StatusCode = _httpCode;
        var body = new MultiStringResponseStream(7);
        body.Builder
            .AppendLine(_headLine)
            .AppendFormat("Host: {0}", request.Host.Host).AppendLine()
            .AppendFormat("Path: {0}", request.Path).AppendLine()
            .AppendFormat("BasePath: {0}", request.PathBase).AppendLine()
            .AppendFormat("Method: {0}", request.Method).AppendLine()
            .AppendFormat("Headers: {0}", string.Join(';', request.Headers.Select(a => $"[{ a.Key}:{ a.Value}]").ToArray())).AppendLine()
            .AppendFormat("Client Ip: {0}", request.HttpContext.Connection.RemoteIpAddress);
        response.Body = body;
        return Task.FromResult(true);
    }
}
