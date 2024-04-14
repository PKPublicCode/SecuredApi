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
namespace SecuredApi.Logic.Routing.Model.Actions.Basic;

/// <summary>
/// Writes client request information to the body of client response. Main usage is debugging and troubleshooting.
/// </summary>
/// <remarks>
/// Action writes to the client response following:
/// 
/// * Host
/// 
/// * Request Path
/// 
/// * Request Path Base
/// 
/// * Method
/// 
/// * Headers
/// 
/// * Inbound IP
/// 
/// </remarks>
public class SetRequestInfoToResponse
{
    /// <summary>
    /// HTTP code set to the client response
    /// </summary>
    /// <value>
    /// 200
    /// </value>
    public int HttpCode { get; init; } = 200;
    /// <summary>
    /// Headline added before request information
    /// </summary>
    /// <value>
    /// "Debug information:"
    /// </value>
    public string HeadLine { get; init; } = "Debug information:";
}
