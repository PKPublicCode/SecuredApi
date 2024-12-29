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
using SecuredApi.Logic.FileAccess;
using SecuredApi.Logic.Common;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace SecuredApi.Infrastructure.FileAccess.FileSystem;

public class FileProvider<T> : IFileProvider<T>
{
    public readonly FileProviderConfig _config;

    public FileProvider(IOptions<FileProviderConfig<T>> config)
    {
        _config = config.Value;
    }

    public Task<FileStreamResult> LoadFileAsync(string fileId, bool includeProps, CancellationToken cancellationToken)
    {
        try
        {
            fileId = Path.Combine(_config.BasePath, fileId);
            var stream = File.OpenRead(fileId);
            
            if (!includeProps)
            {
                return Task.FromResult(new FileStreamResult(stream));
            }

            var props = new FileProps(File.GetLastWriteTimeUtc(fileId).ToString(CultureInfo.InvariantCulture));
            return Task.FromResult(new FileStreamResult(stream, props));
        }
        catch(Exception e) 
            when (e is DirectoryNotFoundException 
                  || e is PathTooLongException
                  || e is NotSupportedException
                  || e is System.IO.FileNotFoundException
                  // On Unix UnauthorizedAccessException is thrown if open folder as file
                  || e is UnauthorizedAccessException)
        {
            throw new Logic.FileAccess.FileNotFoundException("Invalid file id", e);
        }
    }
}
