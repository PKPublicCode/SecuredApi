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
using SecuredApi.Logic.Common;
using SecuredApi.Logic.FileAccess;
using System.Text;

namespace SecuredApi.Logic.Subscriptions;

public abstract class RepositoryTestsBase<T>
    where T: class
{
    protected readonly IFileProvider<T> _fileProvider
        = Substitute.For<IFileProvider<T>>();

    protected readonly T _sut;

    protected RepositoryTestsBase()
    {
        // Return not found for any id except configured by SetupReturn()
        _fileProvider
            .LoadFileAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns<StreamResult>((x) => throw new FileAccess.FileNotFoundException("Not Found"));

        _sut = MakeSut();
    }

    protected abstract T MakeSut();

    protected void SetupReturn(string fileId, string content)
    {
        _fileProvider
            .LoadFileAsync(fileId, Arg.Any<CancellationToken>())
            .Returns(new StreamResult(new MemoryStream(Encoding.UTF8.GetBytes(content))));
    }
}

