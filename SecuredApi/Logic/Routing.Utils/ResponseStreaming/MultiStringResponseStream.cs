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
    public class MultiStringResponseStream: IResponseStream
    {
        public StringBuilder Builder { get; }

        public MultiStringResponseStream(int capacity)
        {
            Builder = new(capacity);
        }

        public MultiStringResponseStream()
        {
            Builder = new();
        }

        public Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            //Completely the same as HttpResponse.WriteAsync does...
            var bytes = Encoding.UTF8.GetBytes(Builder.ToString());
            return destination.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
        }

        public void Dispose()
        {
        }
    }
}
