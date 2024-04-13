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
namespace SecuredApi.Logic.Routing.Actions.Model.Auth;

/// <summary>
/// Verify the subscription key (api key) and checks if subscription is allowed for this route
/// </summary>
/// <return>
/// Action fails if:
/// 
/// * Subscription key header doesn't exist, or empty. In this case it sets response code 401 (Not Authorized)
/// 
/// * Subscription key (api key) is invalid (or doesn't exists). In this case it sets response code 401 (Not Authorized)
///
/// * Subscription key (api key) is valid, but route is not allowed to run. In this case response code set to 401 (Access denied)
/// </return>
public class CheckSubscription
{
    /// <summary>
    /// Header name that bears subscription key
    /// </summary>
    public string SubscriptionKeyHeaderName { get; init; } = null!;

    /// <summary>
    /// Removes this header from the outgoing request
    /// </summary>
    /// <value>true</value>
    public bool SuppressHeader { get; init; } = true;

    /// <summary>
    /// Customized body if key not valid
    /// </summary>
    /// <value>empty string</value>
    public string ErrorNotAuthorizedBody { get; init; } = string.Empty;

    /// <summary>
    /// Customized body if key is valid, but not allowed for this routes group
    /// </summary>
    /// <value>empty string</value>
    public string ErrorAccessDeniedBody { get; init; } = string.Empty;
}
