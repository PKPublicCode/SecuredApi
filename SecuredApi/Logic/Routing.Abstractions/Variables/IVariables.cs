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
using System.Diagnostics.CodeAnalysis;

namespace SecuredApi.Logic.Routing.Variables;

//ToDo.0 Move to variables
public interface IVariables<T>
    where T: class
{
    bool TryGetVariable(string key, [MaybeNullWhen(false)] out T value);
    bool TryGetVariable(ReadOnlySpan<char> key, [MaybeNullWhen(false)] out T value);
    T GetVariable(string key);
    T GetVariable(ReadOnlySpan<char> key);
}

