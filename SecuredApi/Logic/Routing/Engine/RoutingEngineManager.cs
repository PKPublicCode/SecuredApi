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
using SecuredApi.Logic.FileAccess;
using Microsoft.Extensions.Options;
using SecuredApi.Logic.Common;
using SecuredApi.Logic.Variables;

namespace SecuredApi.Logic.Routing.Engine;

public class RoutingEngineManager: IRoutingEngineManager
{
    private readonly IRouterUpdater _routerUpdater;
    private readonly IGlobalVariablesUpdater _variablesUpdater;
    private readonly ILogger _logger;
    private readonly IRoutesParser _routesCfgParser;
    private readonly IGlobalVariablesStreamParser _variablesParser;
    private readonly IDefaultGlobalVariablesProvider _defaultGlobalVariables;
    private readonly RoutingConfigurationFilesCfg _config;
    private readonly IFileProvider _fileProvider;

    public RoutingEngineManager(IRouterUpdater routerUpdater,
                                IGlobalVariablesUpdater variablesUpdater,
                                IRoutesParser routesCfgParser,
                                IGlobalVariablesStreamParser variablesParser,
                                IFileProvider<IRoutingEngineManager> fileProvider,
                                ILogger<RoutingEngineManager> logger,
                                IDefaultGlobalVariablesProvider defVariables,
                                IOptions<RoutingConfigurationFilesCfg> config)
    {
        _routerUpdater = routerUpdater;
        _variablesUpdater = variablesUpdater;
        _logger = logger;
        _fileProvider = fileProvider;
        _routesCfgParser = routesCfgParser;
        _variablesParser = variablesParser;
        _defaultGlobalVariables = defVariables;
        _config = config.Value;
    }

    public async Task InitializeRoutingEngineAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Initializing routing engine...");
        //No transaction support. Not a problem while engine is initialized only once
        //For updating routing engine on the flight (in the future), need implement transactional loading of
        //global variables + Routes
        await LoadGlobalConfigurationAsync(cancellationToken);
        await InitializeRoutesAsync(cancellationToken);
        _logger.LogInformation("Routing engine initialized");
    }

    private async Task LoadGlobalConfigurationAsync(CancellationToken cancellationToken)
    {
        var globalVars = Enumerable.Empty<KeyValuePair<string, string>>();
        if (!string.IsNullOrEmpty(_config.GlobalCfgFileId))
        {
            using var content = await LoadFileConfigurationAsync(_config.GlobalCfgFileId, cancellationToken);
            globalVars = await _variablesParser.ParseAsync(content.Content, cancellationToken);
        }
        
        var defVariables = await _defaultGlobalVariables.GetGlobalVariablesAsync(cancellationToken);
        _variablesUpdater.Update(defVariables.Union(globalVars));
    }

    private async Task InitializeRoutesAsync(CancellationToken cancellationToken)
    {
        using var content = await LoadFileConfigurationAsync(_config.RoutingCfgFileId, cancellationToken);
        var table = await _routesCfgParser.ParseAsync(content.Content, cancellationToken);
        _routerUpdater.UpdateRouter(table);
    }

    private async Task<FileStreamResult> LoadFileConfigurationAsync(string fileId, CancellationToken cancellationToken)
    {
        try
        {
            return await _fileProvider.LoadFileAsync(fileId, cancellationToken);
        }
        catch(Exception e)
        {
            _logger.LogError(e, "Unable to load configuration file");
            throw;
        }
    }
}
