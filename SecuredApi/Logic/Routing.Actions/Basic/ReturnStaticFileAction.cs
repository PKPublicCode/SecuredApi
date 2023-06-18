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
using Microsoft.Extensions.Logging;
using SecuredApi.Logic.Routing.Utils.ResponseStreaming;
using SecuredApi.Logic.FileAccess;
using Path = System.IO.Path;
using SecuredApi.Logic.Common;
using SecuredApi.Logic.Routing.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

namespace SecuredApi.Logic.Routing.Actions.Basic
{
    public interface IStaticFileProvider
    {

    }

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
                path = Path.Combine(path, context.RemainingPath);
            }
            var fileProvider = context.GetRequiredService<IFileProvider<IStaticFileProvider>>();
            StreamResult result;
            try
            {
                result = await fileProvider.LoadFileAsync(path, context.CancellationToken);
            }
            catch(FileNotFoundException)
            {
                await context.SetResponseAsync(StatusCodes.Status404NotFound, _notFoundBody);
                return false;
            }
            catch(Exception e)
                when (!(e is TaskCanceledException))
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
}
