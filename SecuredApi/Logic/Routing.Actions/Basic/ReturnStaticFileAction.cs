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
using SecuredApi.Logic.Routing.Model.Actions.Basic;

namespace SecuredApi.Logic.Routing.Actions.Basic;

public class ReturnStaticFileAction : IAction
{
    private readonly StringResponseStream _notFoundBody;
    private readonly bool _autoMimeType;
    private readonly RuntimeExpression _path;

    public ReturnStaticFileAction(ReturnStaticFile settings)
    {
        _notFoundBody = settings.NotFoundMessage ?? StringResponseStream.Empty;
        _autoMimeType = settings.AutoDiscoverMimeType;
        _path = settings.Path;
    }

    public async Task<bool> ExecuteAsync(IRequestContext context)
    {
        string path = _path.BuildString(context);
        var fileProvider = context.GetRequiredService<FileAccess.IFileProvider<ReturnStaticFileAction>>();
        bool requestFileParams = _autoMimeType;
        string? contentType = null;

        if (requestFileParams)
        {
            requestFileParams = !context.GetRequiredService<IContentTypeProvider>().TryGetContentType(path, out contentType);
        }
        
        FileStreamResult result;
        try
        {
            result = await fileProvider.LoadFileAsync(path, requestFileParams, context.CancellationToken);
        }
        catch(FileAccess.FileNotFoundException)
        {
            await context.SetResponseAsync(StatusCodes.Status404NotFound, _notFoundBody);
            return false;
        }
        catch(Exception e)
            when (e is not TaskCanceledException)
        {
            context.GetLogger<ReturnStaticFileAction>().LogError(e, "Unexpected exception e");
            throw;
        }

        if (requestFileParams)
        {
            contentType = result.Props.ContentType;
        }

        if (!contentType.IsNullOrEmpty())
        {
            context.Response.ContentType = contentType;
        }

        context.Response.Body = new StreamResultResponseStream<FileStreamResult>(result);
        context.Response.StatusCode = StatusCodes.Status200OK;

        return true;
    }
}
