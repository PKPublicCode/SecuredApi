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
using System.Net.Http;
using System.Threading;

namespace SecuredApi.Logic.Routing.Utils.ResponseStreaming
{
    public class HttpResponseMessageStream : IResponseStream
    {
        private readonly HttpResponseMessage _message;
        private Stream? _content;

        public HttpResponseMessageStream(HttpResponseMessage msg)
        {
            _message = msg;
        }

        public async Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            _content ??= await _message.Content.ReadAsStreamAsync(cancellationToken);
            await _content.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        public void Dispose()
        {
            _content?.Dispose();
            _message.Dispose();
        }
    }
}
