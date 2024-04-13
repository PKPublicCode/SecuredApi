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
namespace SecuredApi.Logic.Routing.Actions.OAuth;

/// <summary>
/// Checks claims of the entra jwt. 
/// </summary>
/// <remarks>
/// This action should go only after CheckEntraJwt action. In some cases it's more convenient
/// to CheckEntaJwt for group of routes, but check different claims for different routes in this group.
/// </remarks>
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

