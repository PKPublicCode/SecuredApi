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
using Microsoft.IdentityModel.Tokens;

namespace SecuredApi.Logic.Auth.Jwt;

public static class TokenValidator
{
    public static async Task<ValidationResult> ValidateTokenAsync(string token,
                                ISigningKeysProvider keysProvider,
                                string? issuer,
                                string[]? oneOfAudiences,
                                string[]? oneOfRoles,
                                string[]? oneOfScopes,
                                CancellationToken ct = default
                                )
    {
        var handler = new JsonWebTokenHandler();
        var jwt = handler.ReadJsonWebToken(token);
        var keys = await keysProvider.GetKeysAsync(jwt.Issuer, ct);

        var result = await ValidateMainTokenPropsAsync(jwt, handler, keys, issuer, oneOfAudiences);
        if (result.Succeed)
        {
            result = ValidateClaims(jwt, oneOfRoles, oneOfScopes);
        }

        return result;
    }

    private static async Task<ValidationResult> ValidateMainTokenPropsAsync(JsonWebToken token,
                                        JsonWebTokenHandler handler,
                                        IEnumerable<SecurityKey> keys,
                                        string? issuer,
                                        string[]? oneOfAudiences
                                        )
    {
        var validationParameters = new TokenValidationParameters()
        {
            ValidIssuer = issuer,
            ValidAudiences = oneOfAudiences,
            ValidateLifetime = true,
            ValidateAudience = true,
            ValidateIssuer = true,
            IssuerSigningKeys = keys,
        };

        var res = await handler.ValidateTokenAsync(token, validationParameters);

        if (!res.IsValid)
        {
            return ResultFromException(res.Exception);
        }
        return ValidationResult.MakeOk();
    }

    private static ValidationResult ResultFromException(Exception e) => e switch
    {
        SecurityTokenInvalidIssuerException => ValidationResult.MakeAccessDenied(e),
        _ => ValidationResult.MakeNotAuthorized(e)
    };

    private static ValidationResult ValidateClaims(JsonWebToken token, string[]? oneOfRoles, string[]? oneOfScopes)
    {
        var roles = oneOfRoles ?? Array.Empty<string>();
        var scopes = oneOfScopes ?? Array.Empty<string>();
        bool roleOk = oneOfRoles == null;
        bool scopeOk = oneOfScopes == null;

        bool allOk = roleOk && scopeOk;
        var i = token.Claims.GetEnumerator();
        while (!allOk && i.MoveNext())
        {
            var item = i.Current;
            switch (item.Type)
            {
                case "roles":
                    // roles is an array in json, that interpret by JsonWebToken class as multiple claims with same name
                    roleOk = roleOk || roles.Contains(item.Value);
                    break;
                case "scp":
                    if (!scopeOk) // if scope check required at all
                    {
                        //in contrast of roles, scp claim is string that contains items that split by space
                        foreach (var s in item.Value.Split(" "))
                        {
                            if (scopes.Contains(s))
                            {
                                scopeOk = true;
                                break;
                            }
                        }
                    }
                    break;
            }

            allOk = roleOk && scopeOk;
        }
        
        if (allOk)
        {
            return ValidationResult.MakeOk();
        }

        return ValidationResult.MakeAccessDenied();
    }
}

