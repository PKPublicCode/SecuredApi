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
/// Verifies that JWT is signed by proper keys, has valid issuer and issued for valid audience.
/// </summary>
/// <remarks>
/// Validation is designed for and tested with Entra (Azure AD) json web tokens. Other auth servers're coming.
/// </remarks>
/// <return>
/// Fails in following cases:
/// 
/// I. Not authorized; Set 401 status code to client response:
///
/// * JWT token malformed
///
/// * JWT token doesn't signed by key that correspond to the provided issuer
///
/// II. Access Denied; Sets 403 status code to client response:
/// 
/// * Token issuer is invalid
///
/// * Audience is invalid
///
/// * Roles are invalid
///
/// * Scopes are invalid
/// </return>
public class CheckEntraJwt
{
    /// <summary>
    /// One of allowed issuers
    /// </summary>
    public string[] OneOfIssuers { get; init; } = null!;
    /// <summary>
    /// One of expected audiences
    /// </summary>
    public string[] OneOfAudiences { get; init; } = null!;
    /// <summary>
    /// One of roles that must be in the JWT
    /// </summary>
    /// <value>
    /// null
    /// </value>
    public string[]? OneOfRoles { get; init; }
    /// <summary>
    /// One of scopes that must be in the JWT
    /// </summary>
    /// <value>
    /// null
    /// </value>
    public string[]? OneOfScopes { get; init; }
    /// <summary>
    /// HTTP Header name that bears JWT token
    /// </summary>
    /// <value>
    /// "Authorization"
    /// </value>
    public string HeaderName { get; init; } = "Authorization";
    /// <summary>
    /// Prefix in the header value, that bears JWT token
    /// </summary>
    /// <value>
    /// "Bearer "
    /// </value>
    public string TokenPrefix { get; init; } = "Bearer ";
    /// <summary>
    /// Whether parsed JWT token object should remain in the memory and used by further actions, or can be released.
    /// If CheckEntraJwtClaims action is used later for this route, then value should be set to true.
    /// </summary>
    /// <value>
    /// false
    /// </value>
    public bool KeepData { get; init; } = false;

    /// <summary>
    /// If not null, sets value of the specified claim to ConsumerId,
    /// that can be used later for ```RunConsumerActions``` action
    /// </summary>
    /// <value>
    /// null
    /// </value>
    public string? ConsumerIdClaim { get; init; }
}

