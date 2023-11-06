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
namespace SecuredApi.Logic.Common;

public ref struct DisposeGuard<T>
    where T: class, IDisposable
{
    private T? _value;
    public T Value => _value ?? throw new InvalidOperationException("Object reset");
    public void Reset() => _value = null;
    public void Dispose() => _value?.Dispose();
    public DisposeGuard(T value) => _value = value;
}

public static class DisposableGuard
{
    public static DisposeGuard<T> MakeGuard<T>(this T value)
        where T: class, IDisposable
    {
        return new DisposeGuard<T>(value);
    }
}
