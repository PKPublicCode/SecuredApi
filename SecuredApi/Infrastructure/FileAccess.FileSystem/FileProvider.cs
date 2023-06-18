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
using System.Threading;
using System.Threading.Tasks;
using SecuredApi.Logic.FileAccess;
using SecuredApi.Logic.Common;

namespace SecuredApi.Infrastructure.FileAccess.FileSystem
{
    public class FileProvider<T> : IFileProvider<T>
    {
        public Task<StreamResult> LoadFileAsync(string fileId, CancellationToken cancellationToken)
        {
            try
            {
                return Task.FromResult(new StreamResult(File.OpenRead(fileId)));
            }
            catch(Exception e) 
                when (e is DirectoryNotFoundException 
                      || e is PathTooLongException
                      || e is NotSupportedException
                      || e is System.IO.FileNotFoundException)
            {
                throw new SecuredApi.Logic.FileAccess.FileNotFoundException("Invalid file id", e);
            }
        }
    }
}
