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
using SecuredApi.Logic.Common;

namespace SecuredApi.Logic.Routing.Utils.ResponseStreaming
{
    public class StreamResultResponseStream<T> : IResponseStream
        where T: IStreamResult
    {
        private T _stream; //T could be struct, so don't use readonly to avoid possible side effects.

        public StreamResultResponseStream(T stream)
        {
            _stream = stream;
        }

        public async Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            await _stream.Content.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}
