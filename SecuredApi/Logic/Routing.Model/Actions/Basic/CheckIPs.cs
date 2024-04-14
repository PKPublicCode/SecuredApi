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
using Microsoft.AspNetCore.Http;

namespace SecuredApi.Logic.Routing.Model.Actions.Basic;

/// <summary>
/// Verifies inbound IP address
/// </summary>
/// <remarks>
/// Inbound ip address is taken from the client HTTP request properties.
/// </remarks>
/// <return>
/// Secceeded if IP found in a specified white list. Fails otherwise otherwise
/// </return>
public class CheckIPs
{
    /// <summary>
    /// Array of the allowed IPs
    /// </summary>
    public HashSet<string> WhiteList { get; init; } = null!;

    /// <summary>
    /// Status code returned in case of failure
    /// </summary>
    /// <value>403</value>
    public int NoAccessStatusCode { get; init; } = StatusCodes.Status403Forbidden;

    /// <summary>
    /// Response body returned in case of failure.
    /// </summary>
    /// <value>Empty string</value>
    public string NoAccessResponseBody { get; init; } = string.Empty;
}
