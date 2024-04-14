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
using SecuredApi.Logic.Variables;

namespace SecuredApi.Logic.Routing.Model.Actions.Basic;

/// <summary>
/// Makes outgoing http(s) call to remote service using current state of the client request.
/// Response of the service, including headers, status code and body is saved to the client response
/// </summary>
/// <return>
/// Fails only if timeout occured. Succeeds otherwise.
/// </return>
/// <example>
/// {
///   "type": "RemoteCall",
///   "path": "https://www.google.com/@(requestRemainingPath)",
///   "method": "get"
///   "timeout": 500
/// }
/// </example>
public class RemoteCall
{
    /// <summary>
    /// Url of the downstream service. Allows using runtime variables
    /// </summary>
    public RuntimeExpression Path { get; init; }
    /// <summary>
    /// HTTP Method used to call downstream service
    /// </summary>
    public RuntimeExpression Method { get; init; }
    /// <summary>
    /// Timeout in milliseconds that used for outgoing http call.
    /// If timeout occurs, gateway chain set as failed and status code set to 504 (Gateway timeout)
    /// </summary>
    /// <value>-1 (infinite)</value>
    public int Timeout { get; init; } = -1;
    /// <summary>
    /// If true and remote service replies redirect code, action automatically calls redirected location
    /// and write redirected call to the client response.
    /// 
    /// If false, HTTP Redirect code received from remote server is not validated and is written to client response as is.
    /// Client will be responsible to handle redirect response himself.
    /// </summary>
    /// <value>true</value>
    public bool EnableRedirect { get; init; } = true;
}
