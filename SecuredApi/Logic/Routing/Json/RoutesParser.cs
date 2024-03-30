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
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace SecuredApi.Logic.Routing.Json;

//ToDo.1 Move implementation of RecursiveRoutesParser to this class
public class RoutesParser : IRoutesParser
{
    private readonly IActionFactory _actionFactory;
    private readonly ILogger _logger;
    private readonly IRoutingTableBuilderFactory _routingBuilderFactory;
    private readonly IRoutesParserConfig _jsonConfig;

    public RoutesParser(IActionFactory actionFactory,
                            IRoutesParserConfig jsonConfig,
                            ILogger<RoutesParser> logger,
                            IRoutingTableBuilderFactory routingBuilderFactory
        )
    {
        _actionFactory = actionFactory;
        _logger = logger;
        _routingBuilderFactory = routingBuilderFactory;
        _jsonConfig = jsonConfig;
    }

    public async Task<IRoutingTable> ParseAsync(Stream routeCfg, CancellationToken cancellationToken)
    {
        try
        {
            using var document = await JsonDocument.ParseAsync(routeCfg, _jsonConfig.DocumentOptions, cancellationToken);
            return RecursiveRoutesParser.Parse(document.RootElement, _actionFactory, _jsonConfig, _routingBuilderFactory.Create());
        }
        catch(RouteConfigurationException e)
        {
            _logger.LogError(e, "Error during parsing route configuration file");
            throw;
        }
        catch(Exception e)
        {
            _logger.LogError(e, "Unknown error during loading or parsing rote");
            throw new RouteConfigurationException("Error during loading or parsing route configuration", e);
        }
    }
}
