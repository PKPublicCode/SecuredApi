﻿// Copyright (c) 2021 - present, Pavlo Kruglov.
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
using SecuredApi.Logic.Routing.Utils;
using SecuredApi.Logic.Auth.Jwt;
using Microsoft.IdentityModel.JsonWebTokens;

namespace SecuredApi.Logic.Routing.Actions.OAuth;

public class CheckEntraJwtClaimsAction: IAction
{
    private readonly CheckEntraJwtClaimsActionSettings _settings;

    public CheckEntraJwtClaimsAction(CheckEntraJwtClaimsActionSettings settings)
    {
        _settings = settings;
    }

    public async Task<bool> ExecuteAsync(IRequestContext context)
    {
        if (context.TryGetVariable<JsonWebToken>(VariableNames.Jwt.Token, out var token))
        {
            var result = TokenValidator.ValidateClaims(token, _settings.OneOfRoles, _settings.OneOfScopes);

            if (!result.Succeed)
            {
                return await result.Status.TranslateError(context);
            }

            if (_settings.Cleanup)
            {
                context.Variables.RemoveVariable(VariableNames.Jwt.Token);
            }
            return true;
        }
        return await context.ReturnDataInconsistencyError();
    }
}
