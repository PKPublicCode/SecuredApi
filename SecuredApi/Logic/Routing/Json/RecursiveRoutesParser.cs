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
using System.Text;
using System.Text.Json;
using SecuredApi.Logic.Routing.RequestProcessors;
using static SecuredApi.Logic.Routing.Json.Properties;

namespace SecuredApi.Logic.Routing.Json;

internal class RecursiveRoutesParser
{
    private readonly IRoutingTableBuilder _builder;

    private readonly LinkedList<RoutesGroup> _groups = new();
    private IReadOnlyList<RoutesGroup> _currentGroupsList = _emptyList;
    private readonly ActionsEnumeratorConfig _config;
    private readonly RoutesParserConfig _jsonConfig;

    private RecursiveRoutesParser(IActionFactory actionFactory,
                            RoutesParserConfig jsonConfig,
                            IRoutingTableBuilder builder)
    {
        _builder = builder;
        _jsonConfig = jsonConfig;
        _config = new ActionsEnumeratorConfig()
        {
            ActionFactory = actionFactory,
            SerializerOptions = _jsonConfig.ActionSerializerOptions
        };
    }

    public static IRoutingTable Parse(JsonElement rootJson, IActionFactory actionFactory, RoutesParserConfig jsonConfig, IRoutingTableBuilder builder)
    {
        return new RecursiveRoutesParser(actionFactory, jsonConfig, builder).ParseRoot(rootJson);
    }

    private IRoutingTable ParseRoot(JsonElement rootJson)
    {
        ParseRouteGroup(rootJson);

        _currentGroupsList = _groups.ToList();
        _builder.AddNotFoundRoute(LoadNotFoundActionsRoutingRecord(GetProperty(rootJson, NotFoundRouteActionsPropertyName)));
        return _builder.Build();
    }

    private void ParseRoute(JsonElement routeJson)
    {
        var routeKey = GetProperty<RouteKey>(routeJson, RouteKeyPropertyName);
        var routingRecord = LoadRoutingRecord(routeJson);
        _builder.AddRoute(routeKey.Path, routeKey.Method, routingRecord);
    }

    private void ParseRouteGroup(JsonElement routeGroupJson)
    {
        _groups.AddLast(new RoutesGroup()
        {
            Id = GetGuid(routeGroupJson, RoutesGroupIdPropertyName),
            PreRequestProcessor = LoadRequestProcessorIfExists(routeGroupJson, RoutesGroupPreRequestActions),
            OnRequestErrorProcessor = LoadRequestProcessorIfExists(routeGroupJson, RoutesGroupOnErrorActions),
            OnRequestSuccessProcessor = LoadRequestProcessorIfExists(routeGroupJson, RoutesGroupOnSuccessActions)
        });

        if (routeGroupJson.TryGetProperty(RoutesGroupsPropertyName, out var innerRouteGroupsJson))
        {
            if (routeGroupJson.TryGetProperty(RoutesPropertyName, out var _))
            {
                throw MakeException("Route group can't have both 'routes' and 'routeGroups' properties");
            }

            foreach (var innerRouteGroupJson in innerRouteGroupsJson.EnumerateArray())
            {
                ParseRouteGroup(innerRouteGroupJson);
            }
        }
        else
        {
            if (routeGroupJson.TryGetProperty(RoutesPropertyName, out var routesJson))
            {
                _currentGroupsList = _groups.ToList();
                foreach (var routeJson in routesJson.EnumerateArray())
                {
                    ParseRoute(routeJson);
                }
                _currentGroupsList = _emptyList;
            }
        }

        _groups.RemoveLast();
        
    }

    // Constructs route record for Route Not Found Actions
    private RouteRecord LoadNotFoundActionsRoutingRecord(JsonElement routeJson)
    {
        return new RouteRecord()
        {
            RouteId = Guid.NewGuid(), //Adding fake id, since for not found route it doesn't make any sense
            RequestProcessor = LoadRequestProcessor(routeJson),
            Groups = _currentGroupsList
        };
    }

    private RouteRecord LoadRoutingRecord(JsonElement routeJson)
    {
        return new RouteRecord()
        {
            RouteId = GetGuid(routeJson, RouteIdPropertyName),
            RequestProcessor = LoadRequestProcessor(GetProperty(routeJson, ActionsPropertyName)),
            Groups = _currentGroupsList
        };
    }

    private IRequestProcessor LoadRequestProcessorIfExists(JsonElement json, string propName)
    {
        if (json.TryGetProperty(propName, out var actionsJson))
        {
            return LoadRequestProcessor(actionsJson);
        }
        return EmptyProcessor.Instance;
    }

    private IRequestProcessor LoadRequestProcessor(JsonElement actionsJson)
    {
        int count = actionsJson.GetArrayLength();
        if (count == 0)
        {
            return EmptyProcessor.Instance;
        }

        var actions = new List<IAction>(count);
        try
        {
            foreach (var action in new ActionsEnumerable(actionsJson, _config))
            {
                actions.Add(action);
            }
        }
        catch(RouteConfigurationException e)
        {
            throw MakeException(e);
        }
        return new SequencedRequestProcessor(actions);
    }

    private Guid GetGuid(JsonElement json, string propertyName)
    {
        if (!GetProperty(json, propertyName).TryGetGuid(out var result))
        {
            throw MakeException("Required property is invalid {0}", propertyName);
        }
        return result;
    }

    protected string GetString(JsonElement json, string propertyName)
    {
        var result = GetProperty(json, propertyName).GetString();
        if (result == null)
        {
            throw MakeException("Required property is invalid {0}", propertyName);
        }
        return result;
    }

    private JsonElement GetProperty(JsonElement json, string name)
    {
        if (!json.TryGetProperty(name, out var result))
        {
            throw MakeException("Required property not set: {0}", name);
        }
        return result;
    }

    private T GetProperty<T>(JsonElement json, string name)
    {
        var propJson = GetProperty(json, name);
        return JsonSerializer.Deserialize<T>(propJson.GetRawText(), _jsonConfig.ActionsGroupSerializerOptions)
            ?? throw MakeException("Invalid property: {0}", name);
    }

    private RouteConfigurationException MakeException(string format, string param)
    {
        var sb = new StringBuilder();
        sb.AppendFormat(format, param);
        return MakeException(sb);
    }

    private RouteConfigurationException MakeException(string message)
    {
        var sb = new StringBuilder(message);
        return MakeException(sb);
    }

    private RouteConfigurationException MakeException(RouteConfigurationException e)
    {
        return MakeException(e.Message);
    }

    private RouteConfigurationException MakeException(StringBuilder sb)
    {
        sb.Append("\r\n Path: ");
        sb.AppendJoin("->", _groups.Select(x => "'" + x.Id.ToString() + "'"));
        return new RouteConfigurationException(sb.ToString());
    }

    private readonly struct RouteKey
    {
        public string Method { get; init; }
        public string Path { get; init; }

        public RouteKey(string method, string path)
        {
            Method = method;
            Path = path;
        }
    }

    private static IReadOnlyList<RoutesGroup> _emptyList = new List<RoutesGroup>();
}
