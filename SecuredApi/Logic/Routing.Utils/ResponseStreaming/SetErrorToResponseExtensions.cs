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

namespace SecuredApi.Logic.Routing.Utils.ResponseStreaming
{
    public static class SetErrorToResponseExtensions
    {
        public static Task<bool> SetUnknownErrorAsync(this IRequestContext context, string message)
        {
            return context.SetResponseAsync(StatusCodes.Status500InternalServerError, message);
        }

        public static Task<bool> SetUnknownErrorAsync(this IRequestContext context, IResponseStream body)
        {
            return context.SetResponseAsync(StatusCodes.Status500InternalServerError, body);
        }

        public static Task<bool> ReturnDataInconsistencyError(this IRequestContext context)
        {
            return context.SetUnknownErrorAsync(_internalServerError);
        }

        public static Task<bool> SetAccessDeniedErrorAsync(this IRequestContext httpContext, string message)
        {
            return httpContext.SetResponseAsync(StatusCodes.Status403Forbidden, message);
        }

        public static Task<bool> SetNotAuthorizedErrorAsync(this IRequestContext httpContext, string message)
        {
            return httpContext.SetResponseAsync(StatusCodes.Status401Unauthorized, message);
        }

        public static Task<bool> SetNotAuthorizedErrorAsync(this IRequestContext httpContext, IResponseStream message)
        {
            return httpContext.SetResponseAsync(StatusCodes.Status401Unauthorized, message);
        }

        public static Task<bool> SetAccessDeniedErrorAsync(this IRequestContext httpContext, IResponseStream message)
        {
            return httpContext.SetResponseAsync(StatusCodes.Status403Forbidden, message);
        }

        public static Task<bool> SetResponseAsync(this IRequestContext context, int statusCode, string message)
        {
            return context.SetResponseAsync(statusCode, new StringResponseStream(message));
        }

        public static Task<bool> SetResponseAsync(this IRequestContext context, int statusCode, IResponseStream body)
        {
            context.Response.StatusCode = statusCode;
            context.Response.Body = body;
            return Task.FromResult(false);
        }

        private static readonly StringResponseStream _internalServerError = new ("Internal server error!!! Contact support");
    }
}
