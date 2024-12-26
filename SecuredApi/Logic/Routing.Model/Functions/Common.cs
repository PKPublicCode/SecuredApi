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
public static class Common
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
    /// and receives request with path ```/api/some_feature?a=1&amp;b=2```
    /// then query string will be ```a=1&amp;b=2``
    /// </summary>
    /// <value>getQueryString</value>
    public const string GetQueryString = "getQueryString";
    /// <summary>
    /// Returns parameter of query string. If parameter is not set, returns empty string.
    /// For example, if route was defined for ```/api/some_feature```,
    /// and receives request with path ```/api/some_feature?someParam=1&amp;anotherParam=2```
    /// then query ```getQueryParam(someParam)``` will return ```1```
    /// </summary>
    /// <value>getQueryParam</value>
    public const string GetQueryParam = "getQueryParam";
    /// <summary>
    /// Returns runtime variable.
    /// For example ```getVariable(requestRemainingPath)``` will return the same
    /// string as function ```getRemainingPath()```
    /// </summary>
    /// <value>getVariable</value>
    public const string GetVariable = "getVariable";
    /// <summary>
    /// Rebuilds query string into the new string using new parameter names, beginning,
    /// equality and split characters (strings).
    ///
    /// 
    ///
    /// Signature:
    ///
    /// ``` rebuildQueryString('beginningString', 'splitString', 'equalityString', 'oldParameterName:newParameterName', ...)```
    ///
    /// Number of parameters is limited by 256. Parameters that doesn't have specified name mapping
    /// will be ignored and omitted from output string. Order of the parameters in the result string
    /// satisfies the alphanumerical order of the new parameter names. If query has no parameters with
    /// specified mapping, then resulted string will be empty string
    ///
    ///
    /// 
    /// Example:
    ///
    /// Request's query string: ```?param3=30&amp;param2=20&amp;param1=10```
    ///
    /// Function: ```transformQueryString('_b_', '_s_', '_e_', 'param1:newParam1', 'param2:newParam2'```)
    ///
    /// Result: ```_b_newParam1_e_10_s_newParam2_20```
    /// 
    /// </summary>
    /// <value>transformQueryString</value>
    public const string TransformQueryString = "transformQueryString";
}

