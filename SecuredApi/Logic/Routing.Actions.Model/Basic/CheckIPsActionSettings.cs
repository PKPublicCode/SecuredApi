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

namespace SecuredApi.Logic.Routing.Actions.Model.Basic;

/// <summary>
/// Verifies inbound IP address
/// </summary>
/// <return>True if IP found in a specified white list. False otherwise</return>
public class CheckIPsActionSettings
{
    /// <summary>
    /// Array of the allowed IPs
    /// </summary>
    public HashSet<string> WhiteList { get; init; } = null!;

    /// <summary>
    /// Status code returned in case of failure
    /// </summary>
    /// <default>403</default>
    public int NoAccessStatusCode { get; init; } = StatusCodes.Status403Forbidden;

    /// <summary>
    /// Response body returned in case of failure.
    /// </summary>
    /// <default>Empty string</default>
    public string NoAccessResponseBody { get; init; } = string.Empty;
}
