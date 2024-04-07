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
namespace SecuredApi.Logic.Routing.Actions.Subscriptions;

/// <summary>
/// Verify if subscription key is valid and allowed for this route
/// </summary>
public class CheckSubscriptionActionSettings
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
