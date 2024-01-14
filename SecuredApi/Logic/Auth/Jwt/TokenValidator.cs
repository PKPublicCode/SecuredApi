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
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace SecuredApi.Logic.Auth.Jwt;

public static class TokenValidator
{
    public static ValidationResult ValidateToken(string token,
                                ISigningKeysProvider keysProvider,
                                string? issuer,
                                string[]? oneOfAudiences,
                                string[]? oneOfRoles,
                                string[]? oneOfScopes
                                )
    {
        var result = ParseAndPreValidate(token, keysProvider, issuer, oneOfAudiences, out var jwtToken);
        if (result.Succeed)
        {
            return ValidateClaims(jwtToken!, oneOfRoles, oneOfScopes);
        }

        return result;
    }

    private static ValidationResult ParseAndPreValidate(string tokenStr,
                                        ISigningKeysProvider keysProvider,
                                        string? issuer,
                                        string[]? oneOfAudiences,
                                        out JwtSecurityToken? token
                                        )
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        // public delegate IEnumerable<SecurityKey> IssuerSigningKeyResolver (string token, SecurityToken securityToken, string kid, TokenValidationParameters validationParameters);
        var validationParameters = new TokenValidationParameters()
        {
            ValidIssuer = issuer,
            ValidAudiences = oneOfAudiences,
            ValidateLifetime = true,
            ValidateAudience = true,
            ValidateIssuer = true,
            IssuerSigningKeyResolver = (t, token, kid, p) => {
                return keysProvider.GetKeysAsync(token.Issuer, CancellationToken.None).GetAwaiter().GetResult();
            }
        };

        token = null;

        try
        {
            tokenHandler.ValidateToken(tokenStr, validationParameters, out var result);
            token =  (JwtSecurityToken) result;
        }
        catch (SecurityTokenException e)
            when (e is SecurityTokenExpiredException
                    || e is SecurityTokenSignatureKeyNotFoundException)
        {
            return ValidationResult.MakeNotAuthorized(e);
        }
        catch(SecurityTokenException e)
            when (e is SecurityTokenInvalidIssuerException)
        {
            return ValidationResult.MakeAccessDenied(e);
        }
        catch (SecurityTokenException e)
        {
            // return access denied by default
            return ValidationResult.MakeAccessDenied(e);
        }

        return ValidationResult.MakeOk();
    }

    private static ValidationResult ValidateClaims(JwtSecurityToken token, string[]? oneOfRoles, string[]? oneOfScopes)
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
                    roleOk = roleOk || roles.Contains(item.Value);
                    break;
                case "scp":
                    if (!scopeOk)
                    {
                        var parsedScp = item.Value.Split(" ");
                        foreach (var s in parsedScp)
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

