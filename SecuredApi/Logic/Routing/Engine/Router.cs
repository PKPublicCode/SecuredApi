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
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Collections.Generic;

namespace SecuredApi.Logic.Routing.Engine
{
    public class Router : IRouter, IRouterUpdater
    {
        private IRoutingTable _routingTable = new EmptyRoutingTable();

        public async Task ProcessAsync(HttpContext httpContext)
        {
            var routeInfo = await _routingTable.GetRoutingAsync(httpContext.Request.Path, httpContext.Request.Method, httpContext.RequestAborted);
            var routingRecord = routeInfo.RouteRecord;
            using var processingContext = new RequestContext(routingRecord, httpContext);

            //ToDo.0 Improve and move to variables
            processingContext.Variables.SetVariable("requestRemainingPath", routeInfo.RemainingPath);
            processingContext.Variables.SetVariable("requestHttpMethod", httpContext.Request.Method);
            processingContext.Variables.SetVariable("requestQueryString", httpContext.Request.QueryString.ToString());

            if (await ProcessGroupsActionsAsync(routingRecord.Groups, processingContext, _executePreRequest)
                && await routingRecord.RequestProcessor.ProcessRequestAsync(processingContext))
            {
                await ProcessReversedGroupsActionsAsync(routingRecord.Groups, processingContext, _executeOnSuccess);
            }
            else
            {
                await ProcessReversedGroupsActionsAsync(routingRecord.Groups, processingContext, _executeOnError);
            }
            await SendResponseAsync(processingContext);
        }

        public void UpdateRouter(IRoutingTable routingTree)
        {
            Interlocked.Exchange(ref _routingTable, routingTree);
        }

        private const int _streamCopyBufferSize = 81920;
        private delegate Task<bool> ExecuteDelegate(RoutesGroup group, RequestContext procContext);
        private static readonly ExecuteDelegate _executeOnSuccess = (g, p) => g.OnRequestSuccessProcessor.ProcessRequestAsync(p);
        private static readonly ExecuteDelegate _executeOnError = (g, p) => g.OnRequestErrorProcessor.ProcessRequestAsync(p);
        private static readonly ExecuteDelegate _executePreRequest = (g, p) => g.PreRequestProcessor.ProcessRequestAsync(p);

        private static Task SendResponseAsync(RequestContext context)
        {
            return context.Response.Body?.CopyToAsync(context.HttpContext.Response.Body, _streamCopyBufferSize, context.HttpContext.RequestAborted)
                ?? Task.CompletedTask;
        }

        private static async Task<bool> ProcessReversedGroupsActionsAsync(
                                            IReadOnlyList<RoutesGroup> groups, 
                                            RequestContext procContext,
                                            ExecuteDelegate exec)
        {
            for (int i = groups.Count - 1; i >= 0; --i)
            {
                if (!await exec(groups[i], procContext))
                {
                    return false;
                }
            }
            return true;
        }

        private static async Task<bool> ProcessGroupsActionsAsync(
                                            IReadOnlyList<RoutesGroup> groups,
                                            RequestContext procContext,
                                            ExecuteDelegate exec)
        {
            foreach(var g in groups)
            {
                if (!await exec(g, procContext))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
