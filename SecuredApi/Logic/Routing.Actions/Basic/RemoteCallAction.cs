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

namespace SecuredApi.Logic.Routing.Actions.Basic;

public class RemoteCallAction: IAction
{
    private readonly Uri _uri;
    private readonly HttpMethod? _method;
    private readonly TimeSpan _timeout;
    private readonly string _clientName;

    public RemoteCallAction(RemoteCallActionSettings config)
    {
        _uri = new Uri(config.Path);

        //Shortcut to make runtime variable (requestHttpMethod) work.
        //Later will rewrite\redesign code for more generic use of runntime variables
        if (config.Method != "@(requestHttpMethod)")
        {
            _method = new(config.Method);
        }

        if(config.Timeout == -1)
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
        using var message = CreateEndpointRequestMessage(context.RemainingPath, context.Request);
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

    private HttpRequestMessage CreateEndpointRequestMessage(string remainingPath, HttpRequest request)
    {
        var message = new HttpRequestMessage();

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

        var builder = new UriBuilder(_uri);
        if (remainingPath.Length > 0  && remainingPath[0] != '/')
        {
            builder.Path += '/';
        }
        builder.Path += remainingPath;
        builder.Query += request.QueryString;
        message.RequestUri = builder.Uri;

        //Shortcut to make runtime variable (requestHttpMethod) work.
        //Later will rewrite\redesign code for more generic use of runntime variables
        message.Method = _method ?? new HttpMethod(request.Method);

        return message;
    }

    private static readonly StringResponseStream _upstreamRequestTimeout = new("Gateway timout. Upstream service didn't respond timely");
    private static readonly StringResponseStream _upstreamServiceUnavailable = new("Bad gateway. Upstream service unavailable.");
}
