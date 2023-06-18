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
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Text;

namespace SecuredApi.Logic.Routing.Utils.ResponseStreaming
{
    public class StringResponseStream : IResponseStream
    {
        private readonly byte[] _body;

        public StringResponseStream(string body)
        {
            _body = Encoding.UTF8.GetBytes(body);
        }

        protected StringResponseStream()
        {
            _body = Array.Empty<byte>();
        }

        public static implicit operator StringResponseStream(string body) => new(body);

        public Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            return destination.WriteAsync(_body, 0, _body.Length, cancellationToken);
        }

        public void Dispose()
        {
        }

        public readonly static StringResponseStream Empty = new();
    }
}
