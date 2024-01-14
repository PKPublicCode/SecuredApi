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
using Microsoft.IdentityModel.JsonWebTokens;

namespace SecuredApi.Logic.Auth.Jwt;

public readonly struct ValidationResult
{
    public ValidationStatus Status { get; }
    public Exception? ErrorMessage { get; }
    public JsonWebToken? Jwt { get; }
    public bool Succeed => Status == ValidationStatus.Ok;

    public ValidationResult(ValidationStatus status, JsonWebToken? jwt = null, Exception? e = null)
    {
        Status = status;
        ErrorMessage = e;
        Jwt = jwt;
    }

    public static ValidationResult MakeOk(JsonWebToken jwt)
    {
        return new ValidationResult(ValidationStatus.Ok, jwt);
    }

    public static ValidationResult MakeAccessDenied(Exception? e = null)
    {
        return new ValidationResult(ValidationStatus.AccessDenied, e: e);
    }

    public static ValidationResult MakeNotAuthorized(Exception? e = null)
    {
        return new ValidationResult(ValidationStatus.NotAuthorized, e: e);
    }
}

