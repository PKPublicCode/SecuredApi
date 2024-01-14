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
using SecuredApi.Logic.Auth.Jwt;
using SecuredApi.Logic.Routing.Utils.ResponseStreaming;

namespace SecuredApi.Logic.Routing.Actions.OAuth;

public static class ValidationResultExtensions
{
    public static readonly StringResponseStream NotAuthorized = new("Not Authorized");
    public static readonly StringResponseStream NotAllowed = new("Access Denied");

    public static async Task<bool> TranslateError(this ValidationStatus status, IRequestContext context)
    {
        if (status == ValidationStatus.NotAuthorized)
        {
            return await context.SetNotAuthorizedErrorAsync(NotAuthorized);
        }
        else if (status == ValidationStatus.AccessDenied)
        {
            return await context.SetAccessDeniedErrorAsync(NotAllowed);
        }
        throw new InvalidOperationException("Validation status is not an error");
    }
}

