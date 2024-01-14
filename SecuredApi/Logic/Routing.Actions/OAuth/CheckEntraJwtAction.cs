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
using System.Diagnostics.CodeAnalysis;
using SecuredApi.Logic.Routing.Utils;
using SecuredApi.Logic.Auth.Jwt;

namespace SecuredApi.Logic.Routing.Actions.OAuth;

public class CheckEntraJwtAction : IAction
{
    private readonly CheckEntraJwtActionSettings _settings;

    public CheckEntraJwtAction(CheckEntraJwtActionSettings settings)
    {
        _settings = settings;
    }

    public async Task<bool> ExecuteAsync(IRequestContext context)
    {
        if (TryGetToken(context, out var strToken))
        {
            var result = await TokenValidator.ValidateTokenAsync(strToken, context.GetRequiredService<ISigningKeysProvider>(),
                                            _settings.OneOfIssuers, _settings.OneOfAudiences,
                                            _settings.OneOfRoles, _settings.OneOfScopes);

            if(!result.Succeed)
            {
                return await result.Status.TranslateError(context);
            }

            if (_settings.KeepData)
            {
                context.Variables.SetVariable(VariableNames.Jwt.Token, result.Jwt!);
            }
            return true;
        }

        return await context.SetNotAuthorizedErrorAsync(ValidationResultExtensions.NotAuthorized);
    }

    private bool TryGetToken(IRequestContext context, [MaybeNullWhen(false)] out string value)
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

