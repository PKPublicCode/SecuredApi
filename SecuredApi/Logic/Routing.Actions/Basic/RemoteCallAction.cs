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
using Microsoft.Extensions.DependencyInjection;
using SecuredApi.Logic.Routing.Utils.ResponseStreaming;
using SecuredApi.Logic.Routing.Utils;
using SecuredApi.Logic.Variables;
using SecuredApi.Logic.Routing.Model.Actions.Basic;

namespace SecuredApi.Logic.Routing.Actions.Basic;

public class RemoteCallAction: IAction
{
    private readonly RuntimeExpression _path;
    private readonly Uri? _constPath;
    private readonly RuntimeExpression _method;
    private readonly HttpMethod? _constMethod;
    private readonly TimeSpan _timeout;
    private readonly string _clientName;

    public RemoteCallAction(RemoteCall config)
    {
        _path = config.Path;
        _method = config.Method;

        //Optimization to cache immutable objects in memory
        if (_path.Immutable)
        {
            _constPath = new Uri(_path.ImmutableValue);
        }

        if (_method.Immutable)
        {
            _constMethod = new(_method.ImmutableValue);
        }

        if (config.Timeout == -1)
        {
            _timeout = Timeout.InfiniteTimeSpan;
        }
        else
        {
            _timeout = TimeSpan.FromMilliseconds(config.Timeout);
        }

        _clientName = HttpClientNames.GetClientName(config.EnableRedirect);
    }

    public async Task<bool> ExecuteAsync(IRequestContext context)
    {
        var httpClient = context.GetRequiredService<IHttpClientFactory>().CreateClient(_clientName);
        httpClient.Timeout = _timeout;
        using var message = CreateEndpointRequestMessage(context);
        try
        {
            // endpointReponse is IDisposable, however it can't be dispossed here.
            // It will be attached to context.Response, and disposed later. EndpointResponseToContextResponse shouldn't
            // throw any exceptions, so error check in try-finally and dispose is not required.
            var endpointResponse = await httpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, context.CancellationToken);
            EndpointResponseToContextResponse(endpointResponse, context.Response);
            return true;
        }
        catch (TaskCanceledException e) when (e.InnerException is TimeoutException)
        {
            return await context.SetResponseAsync(StatusCodes.Status504GatewayTimeout, _upstreamRequestTimeout);
        }
        catch (HttpRequestException)
        {
            return await context.SetResponseAsync(StatusCodes.Status502BadGateway, _upstreamServiceUnavailable);
        }
    }

    private static void EndpointResponseToContextResponse(HttpResponseMessage message, IResponse response)
    {
        response.StatusCode = (int)message.StatusCode;
        foreach (var header in message.Headers)
        {
            response.Headers[header.Key] = header.Value.ToArray();
        }

        foreach (var header in message.Content.Headers)
        {
            response.Headers[header.Key] = header.Value.ToArray();
        }

        // SendAsync removes chunking from the response. This removes the header so it doesn't expect a chunked response.
        response.Headers.Remove("transfer-encoding");

        response.Body = new HttpResponseMessageStream(message);
    }

    private HttpRequestMessage CreateEndpointRequestMessage(IRequestContext context)
    {
        var message = new HttpRequestMessage();
        var request = context.Request;

        if (request.ContentLength > 0)
        {
            message.Content = new StreamContent(request.Body);
        }

        // Copy the request headers
        foreach (var header in request.Headers)
        {
            if (!message.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()) && message.Content != null)
            {
                message.Content.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }
        }
        //Host header is calculated according to destination.
        //Since destination is changed, previous Host value is not valid
        message.Headers.Remove("Host");

        message.RequestUri = _constPath
            ?? new Uri(_path.BuildString(context));
        message.Method = _constMethod
            ?? new HttpMethod(_method.BuildString(context));

        return message;
    }

    private static readonly StringResponseStream _upstreamRequestTimeout = new("Gateway timout. Upstream service didn't respond timely");
    private static readonly StringResponseStream _upstreamServiceUnavailable = new("Bad gateway. Upstream service unavailable.");
}
