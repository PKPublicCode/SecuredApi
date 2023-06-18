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
using System;
using System.IO;

namespace SecuredApi.Logic.Common
{
    public struct StreamResult: IDisposable, IStreamResult
    {
        public Stream  Content { get; init; }
        private readonly IDisposable? _parent;

        public StreamResult(Stream content, IDisposable? parent = null)
        {
            Content = content;
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
}
