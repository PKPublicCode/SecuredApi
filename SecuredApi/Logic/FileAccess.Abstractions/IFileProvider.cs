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
using System.Threading;
using System.Threading.Tasks;
using SecuredApi.Logic.Common;

namespace SecuredApi.Logic.FileAccess
{
    public interface IFileProvider
    {
        Task<FileStreamResult> LoadFileAsync(string fileId, CancellationToken cancellationToken)
            => LoadFileAsync(fileId, false, cancellationToken);
        
        Task<FileStreamResult> LoadFileAsync(string fileId, bool includeProps, CancellationToken cancellationToken);
    }

    public interface IFileProvider<T>: IFileProvider
    {
    }
}
