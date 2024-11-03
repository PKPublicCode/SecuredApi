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
namespace SecuredApi.Logic.Routing.Model.Functions;

/// <summary>
/// Functions to access runtime data
/// </summary>
public static class Request
{
    /// <summary>
    /// Returns remained path that was captured by * (asterisk) for the rote.
    /// For example, if route was defined for ```/api/some_feature/*```,
    /// and received request with path ```/api/some_feature/a/b/c/d```
    /// then remained path will be ```a/b/c/d```
    /// </summary>
    /// <value>getRemainingPath</value>
    public const string GetRemainingPath = "getRemainingPath";
    /// <summary>
    /// Returns Http Method of client request
    /// </summary>
    /// <value>getRequestMethod</value>
    public const string GetHttpMethod = "getRequestMethod";
    /// <summary>
    /// Returns query string of the client request.
    /// For example, if route was defined for ```/api/some_feature```,
    /// and received request with path ```/api/some_feature?a=1&amp;b=2```
    /// then query string will be ```a=1&amp;b=2``
    /// </summary>
    /// <value>getQueryString</value>
    public const string GetQueryString = "getQueryString";
    /// <summary>
    /// Returns parameter of query string. If parameter is not set, returns empty string.
    /// For example, if route was defined for ```/api/some_feature```,
    /// and received request with path ```/api/some_feature?a=1&amp;b=2```
    /// then query string will be ```a=1&amp;b=2``
    /// </summary>
    /// <value>getQueryParam</value>
    public const string GetQueryParam = "getQueryParam";
    /// <summary>
    /// Returns runtime variable.
    /// For example ```getVariable(requestRemainingPath)``` will return the same
    /// string as function ```getRemainingPath()```
    /// </summary>
    /// <value>getQueryParam</value>
    public const string GetVariable = "getVariable";
}

