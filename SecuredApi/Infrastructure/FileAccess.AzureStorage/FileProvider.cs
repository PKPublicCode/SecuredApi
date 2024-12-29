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
using Microsoft.Extensions.Options;
using Azure.Storage.Blobs;
using Azure.Identity;
using SecuredApi.Logic.Common;

namespace SecuredApi.Infrastructure.FileAccess.AzureStorage;

public class FileProvider<T> : IFileProvider<T>
{
    private readonly BlobContainerClient _client;

    public FileProvider(IOptions<FileProviderConfig<T>> config)
    {
        var rbac = config.Value.Rbac
            ?? throw new InvalidOperationException("Azure Storage File Access (Rbac) is not configured");
        _client = new BlobContainerClient(new Uri(rbac.Uri), new DefaultAzureCredential());
    }

    public async Task<FileStreamResult> LoadFileAsync(string fileId, bool includeProps, CancellationToken cancellationToken)
    {
        try
        {
            var client = _client.GetBlobClient(fileId);
            var blob = await client.DownloadStreamingAsync(cancellationToken: cancellationToken);

            FileProps props = default;
            if (includeProps)
            {
                var blobProps = await client.GetPropertiesAsync(null, cancellationToken);
                props = new(blobProps.Value.ETag.ToString(), blobProps.Value.ContentType);
            }
            
            //Need to dispose blob.Value (BlobDownloadStreamingResult: IDisposable), however it will dispose Content (Stream)
            //As a workaround return StreamResult, that keep reference to 'parent disposable'
            return new FileStreamResult(blob.Value.Content, props, blob.Value);
        }
        catch(Exception e)
            when (e is not TaskCanceledException)
        {
            //Cutting corner. Need to differentiate exceptions for FileNotFound, AccessDenied, etc.
            throw new Logic.FileAccess.FileNotFoundException("Unable to access file", e);
        }
    }
}
