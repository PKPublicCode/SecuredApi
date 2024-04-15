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
namespace SecuredApi.Logic.Routing.Model.Actions.Auth;

/// <summary>
/// Checks claims of the Entra jwt. 
/// </summary>
/// <remarks>
/// This action should go only after CheckEntraJwt action. In some cases it's more convenient
/// to CheckEntraJwt for group of routes, but check different claims for different routes in this group.
/// </remarks>
/// <return>
/// Fails if JWT doesn't satisfy one of roles, or one of scopes specified in the parameters.
/// In this case sets http code to 403 (access denied) in the client response.
/// </return>
public class CheckEntraJwtClaims
{
    /// <summary>
    /// Sets one of roles that must be set in the JWT
    /// </summary>
    /// <value>null</value>
    public string[]? OneOfRoles { get; init; }
    /// <summary>
    /// Sets one of scopes that must be set the JWT
    /// </summary>
    /// <value>null</value>
    public string[]? OneOfScopes { get; init; }
    /// <summary>
    /// If true, releases parsed JWT object and frees memory
    /// </summary>
    /// <value>true</value>
    public bool Cleanup = true;
}

