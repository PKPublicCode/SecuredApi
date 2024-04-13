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
using SecuredApi.Logic.Variables;

namespace SecuredApi.Logic.Routing.Actions.Model.Basic;

/// <summary>
/// Returns static content (file) to the client.
/// </summary>
/// <remarks>
/// Files can be stored either on the file system or in the storage account.
/// See StaticFileProvider configuration
/// </remarks>
/// <return>
/// Fails if file not found. Set HTTP Code 404 to client response in this case
/// </return>
public class ReturnStaticFile
{
    /// <summary>
    /// Relative path to the file.
    /// </summary>
    /// <remarks>
    /// Allows using runtime variables
    /// </remarks>
    public RuntimeExpression Path { get; init; }
    /// <summary>
    /// String that is written to the client response body if file wasn't found
    /// </summary>
    /// <value>empty string</value>
    public string NotFoundMessage { get; init; } = string.Empty;
    /// <summary>
    /// If set to true, tries automatically discover mime type depending on the file name and adds appropriate header to client response.
    /// For more details read about [IContentTypeProvider](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.staticfiles.icontenttypeprovider?view=aspnetcore-7.0)
    /// </summary>
    /// <value>true</value>
    public bool AutoDiscoverMimeType { get; init; } = true;
}
