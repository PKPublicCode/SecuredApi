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
using Microsoft.Extensions.Logging;
using SecuredApi.Logic.Routing.Utils.ResponseStreaming;
using SecuredApi.Logic.Common;
using SecuredApi.Logic.Routing.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

namespace SecuredApi.Logic.Routing.Actions.Basic;

public class ReturnStaticFileAction : IAction
{
    private readonly string _fullPath;
    private readonly bool _singleFile;
    private readonly StringResponseStream _notFoundBody;
    private readonly bool _autoMimeType;

    public ReturnStaticFileAction(ReturnStaticFileActionSettings settings)
    {
        _fullPath = Path.Combine(settings.Path.Safe(), settings.FileName.Safe());
        _singleFile = !settings.FileName.IsNullOrEmpty();
        _notFoundBody = settings.NotFoundMessage ?? StringResponseStream.Empty;
        _autoMimeType = settings.AutoDiscoverMimeType;
    }

    public async Task<bool> ExecuteAsync(IRequestContext context)
    {
        string path = _fullPath;
        if (!_singleFile)
        {
            // Need to add this check because empty remaining path means trying to open folder as file.
            // This can cause some side effects. E.g. on MacOs (at least) it will cause AccessDenied (UnauthorizedAccessException) exception
            // (meaning that attempting to open folder as file causes UnauthorizedAccessException)
            // So, just returning 404 in this case on this level, since it's kind of logical edge case, whatewer concret file provider is implemented
            if (context.RemainingPath.IsNullOrEmpty())
            {
                await context.SetResponseAsync(StatusCodes.Status404NotFound, _notFoundBody);
                return false;
            }
            path = Path.Combine(path, context.RemainingPath);
        }
        var fileProvider = context.GetRequiredService<FileAccess.IFileProvider<ReturnStaticFileAction>>();
        StreamResult result;
        try
        {
            result = await fileProvider.LoadFileAsync(path, context.CancellationToken);
        }
        catch(FileAccess.FileNotFoundException)
        {
            await context.SetResponseAsync(StatusCodes.Status404NotFound, _notFoundBody);
            return false;
        }
        catch(Exception e)
            when (e is not TaskCanceledException)
        {
            context.GetLogger<ReturnStaticFileAction>().LogError("Unexpected exception e", e);
            throw;
        }

        if (_autoMimeType 
            && context.GetRequiredService<IContentTypeProvider>().TryGetContentType(path, out var type))
        {
            context.Response.ContentType = type;
        }

        context.Response.Body = new StreamResultResponseStream<StreamResult>(result);
        context.Response.StatusCode = StatusCodes.Status200OK;

        return true;
    }
}
