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
namespace SecuredApi.Logic.Routing.Model.RuntimeVariables;

/// <summary>
/// Variables set by routing engine for this request and route
/// </summary>
public static class Request
{
    /// <summary>
    /// Remained path that was captured by * (asterisk) for the rote.
    /// For example, if route was defined for ```/api/some_feature/*```,
    /// and received request with path ```/api/some_feature/a/b/c/d```
    /// then remained path will be ```a/b/c/d```
    /// </summary>
    /// <value>requestRemainingPath</value>
    public const string RemainingPath = "requestRemainingPath";
}

