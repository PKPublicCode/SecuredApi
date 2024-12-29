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

public struct FileStreamResult: IDisposable, IStreamResult
{
    public Stream  Content { get; }
    public FileProps Props { get; }
        
    private readonly IDisposable? _parent;

    public FileStreamResult(Stream content, FileProps props = default, IDisposable? parent = null)
    {
        Content = content;
        Props = props;
        _parent = parent;
    }

    public void Dispose()
    {
        if (_parent != null)
        {
            _parent.Dispose();
        }
        else
        {
            Content.Dispose();
        }
    }
}
