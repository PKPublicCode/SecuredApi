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
using SecuredApi.Logic.Routing.Utils.ResponseStreaming;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using SecuredApi.Logic.Routing.Utils;

namespace SecuredApi.Logic.Routing.Actions.OAuth;

public class CheckEntraTokenAction : IAction
{
    private static readonly StringResponseStream _notAuthorized = new("Not Authorized");
    private static readonly StringResponseStream _notAllowed = new("Access denied");

    private readonly CheckEntraTokenActionSettings _settings;

    public CheckEntraTokenAction(CheckEntraTokenActionSettings settings)
    {
        _settings = settings;
    }

    public async Task<bool> ExecuteAsync(IRequestContext context)
    {
        if (!TryGetToken(context, out var strToken))
        {
            return await context.SetNotAuthorizedErrorAsync(_notAuthorized);
        }

        var keys = await context.GetRequiredService<ISigningKeysProvider>().GetKeysAsync(context.CancellationToken);

        if (!BasicTokenValidation(context, strToken!, keys, out var jwtToken))
        {
            return await context.SetAccessDeniedErrorAsync(_notAllowed);
        }

        return true;
    }

    private bool BasicTokenValidation(IRequestContext context, string tokenStr, IEnumerable<SecurityKey> keys, [MaybeNullWhen(false)] out JwtSecurityToken? jwtToken)
    {
        jwtToken = default;
        var result = false;
        var tokenHandler = new JwtSecurityTokenHandler();

        var validationParameters = new TokenValidationParameters()
        {
            ValidIssuer = _settings.Issuer,
            ValidAudience = _settings.Audience,
            ValidateLifetime = true,
            ValidateAudience = true,
            ValidateIssuer = true,
            IssuerSigningKeys = keys,
        };

        try
        {
            var principal = tokenHandler.ValidateToken(tokenStr, validationParameters, out var token);
            jwtToken = (JwtSecurityToken)token;
        }
        catch (SecurityTokenException ex)
        {
            result = false;
        }

        return result;
    }

    private bool TryGetToken(IRequestContext context, [MaybeNullWhen(false)] out string? value)
    {
        if (context.Request.Headers.TryGetValue(_settings.HeaderName, out var header)
            && header.Count > 0)
        {
            var token = header.Where(x => !string.IsNullOrEmpty(x)).FirstOrDefault();
            if (token != null && token.StartsWith(_settings.TokenPrefix))
            {
                value = token.Substring(_settings.TokenPrefix.Length);
                return true;
            }
        }

        value = default;
        return false;
    }


}

