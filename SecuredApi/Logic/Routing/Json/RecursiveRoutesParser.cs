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
using SecuredApi.Logic.Variables;
using static System.Text.Json.JsonElement;
using static SecuredApi.Logic.Routing.Json.Properties;

namespace SecuredApi.Logic.Routing.Json;

internal class RecursiveRoutesParser
{
    private readonly IRoutingTableBuilder _builder;

    private readonly LinkedList<RoutesGroup> _groups = new();
    private readonly ActionsEnumeratorConfig _config;
    private readonly IRoutesParserConfig _jsonConfig;
    private readonly LinkedList<string?> _urlPath = new();
    private LinkedList<string> _errorTracePath = new();

    private RecursiveRoutesParser(IActionFactory actionFactory,
                            IRoutesParserConfig jsonConfig,
                            IRoutingTableBuilder builder)
    {
        _builder = builder;
        _jsonConfig = jsonConfig;
        _config = new ActionsEnumeratorConfig()
        {
            ActionFactory = actionFactory,
            SerializerOptions = _jsonConfig.SerializerOptions
        };
    }

    public static IRoutingTable Parse(JsonElement rootJson, IActionFactory actionFactory, IRoutesParserConfig jsonConfig, IRoutingTableBuilder builder)
    {
        return new RecursiveRoutesParser(actionFactory, jsonConfig, builder).ParseRoot(rootJson);
    }

    private IRoutingTable ParseRoot(JsonElement rootJson)
    {
        ParseRouteGroup(rootJson);
        _builder.AddNotFoundRoute(LoadNotFoundActionsRoutingRecord(rootJson));
        return _builder.Build();
    }

    private void ParseRoute(JsonElement routeJson,
                            IReadOnlyList<RoutesGroup> groups,
                            IReadOnlySet<Guid> groupIds)
    {
        var routingRecord = LoadRoutingRecord(routeJson, groups, groupIds);
        var fullPath = MakeFullUrlPath(GetString(routeJson, RoutePath));
        foreach (var method in EnumerateRequiredProperty(routeJson, RouteMethods))
        {
            _builder.AddRoute(
                          fullPath,
                          method.ToString(),
                          routingRecord
                         );
        }
    }

    private string MakeFullUrlPath(string end)
    {
        //ToDo: Not efficient concatenation for every route. Will rewrite later
        return string.Concat(string.Concat(_urlPath.Where(s => !string.IsNullOrEmpty(s))),
                            end);
    }

    private void ParseRouteGroup(JsonElement routeGroupJson)
    {
        ParseAndPushRoutesGroupObject(routeGroupJson);

        if (routeGroupJson.TryGetProperty(RoutesGroups, out var innerRouteGroupsJson))
        {
            if (routeGroupJson.TryGetProperty(Routes, out var _))
            {
                throw MakeException($"Route group can't have both {Routes} and {RoutesGroups} properties");
            }

            foreach (var innerRouteGroupJson in innerRouteGroupsJson.EnumerateArray())
            {
                ParseRouteGroup(innerRouteGroupJson);
            }
        }
        else if (routeGroupJson.TryGetProperty(Routes, out var routesJson))
        {
            var groups = _groups.ToList();
            var groupIds = MakeSet(groups);
            foreach (var routeJson in routesJson.EnumerateArray())
            {
                ParseRoute(routeJson, groups, groupIds);
            }
        }
        else
        {
            throw MakeException($"Niether {Routes}, nor {RoutesGroups} properties set. One of them has to be configured");
        }

        PopRoutesGroupObject();
        
    }

    private void ParseAndPushRoutesGroupObject(JsonElement routeGroupJson)
    {
        _groups.AddLast(new RoutesGroup()
        {
            Id = GetOptionalGuid(routeGroupJson, RoutesGroupId),
            PreRequestProcessor = LoadRequestProcessorIfExists(routeGroupJson, RoutesGroupPreRequestActions),
            OnRequestErrorProcessor = LoadRequestProcessorIfExists(routeGroupJson, RoutesGroupOnErrorActions),
            OnRequestSuccessProcessor = LoadRequestProcessorIfExists(routeGroupJson, RoutesGroupOnSuccessActions)
        });

        _urlPath.AddLast(GetOptionalString(routeGroupJson, RoutesGroupPath));

        _errorTracePath.AddLast(_groups.Last!.Value.Id?.ToString()
                                ?? GetOptionalString(routeGroupJson, RoutesGroupDescription)
                                ?? "Unknown");
    }

    private void PopRoutesGroupObject()
    {
        _groups.RemoveLast();
        _urlPath.RemoveLast();
        _errorTracePath.RemoveLast();
    }

    // Constructs route record for Route Not Found Actions
    private RouteRecord LoadNotFoundActionsRoutingRecord(JsonElement rootJson)
    {
        var routeJson = GetProperty(rootJson, RootRoutesGroupNotFoundRouteActions);
        return new RouteRecord
        (
            RequestProcessor: LoadRequestProcessor(routeJson),
            Groups: _emptyGroups,
            GroupIds: _emptyIds
        );
    }

    private RouteRecord LoadRoutingRecord(JsonElement routeJson,
                                        IReadOnlyList<RoutesGroup> groups,
                                        IReadOnlySet<Guid> groupIds)
    {
        return new RouteRecord
        (
            RouteId: GetOptionalGuid(routeJson, RouteId),
            RequestProcessor: LoadRequestProcessor(GetProperty(routeJson, Actions)),
            Groups: groups,
            GroupIds: groupIds
        );
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
        catch(Exception e)
            when (e is RouteConfigurationException
                || e is InvalidExpressionException)
        {
            throw MakeException(e);
        }
        return new SequencedRequestProcessor(actions);
    }

    private Guid? GetOptionalGuid(JsonElement json, string propertyName)
    {
        if (json.TryGetProperty(propertyName, out var prop))
        {
            if (prop.TryGetGuid(out var result))
            {
                return result;
            }
            else
            {
                throw MakeException("Invalid GUID value in property {0}", propertyName);
            }
        }

        return null;
    }

    private string GetString(JsonElement json, string propertyName)
    {
        return GetOptionalString(json, propertyName)
            ?? throw MakeException("Required property is invalid {0}", propertyName);
    }

    private string? GetOptionalString(JsonElement json, string propertyName)
    {
        return GetOptionalProperty(json, propertyName)?.GetString();
    }

    private ArrayEnumerator EnumerateRequiredProperty(JsonElement json, string name)
    {
        var prop = GetProperty(json, name);
        try
        {
            return prop.EnumerateArray();
        }
        catch (InvalidOperationException e)
        {
            throw MakeException("Invalid property type {0}", name, e);
        }
    }

    private JsonElement? GetOptionalProperty(JsonElement json, string propertyName)
    {
        if (json.TryGetProperty(propertyName, out var result))
        {
            return result;
        }
        return null;
    }

    private JsonElement GetProperty(JsonElement json, string name)
    {
        return GetOptionalProperty(json, name)
            ?? throw MakeException("Required property not set: {0}", name);
    }

    private T GetProperty<T>(JsonElement json, string name)
    {
        var propJson = GetProperty(json, name);
        return JsonSerializer.Deserialize<T>(propJson.GetRawText(), _jsonConfig.SerializerOptions)
            ?? throw MakeException("Invalid property: {0}", name);
    }

    private RouteConfigurationException MakeException(string format, string param, Exception? inner = null)
    {
        var sb = new StringBuilder();
        sb.AppendFormat(format, param);
        return MakeException(sb, inner);
    }

    private RouteConfigurationException MakeException(string message, Exception? inner = null)
    {
        var sb = new StringBuilder(message);
        return MakeException(sb, inner);
    }

    private RouteConfigurationException MakeException(Exception e)
    {
        return MakeException(e.Message, e);
    }

    private RouteConfigurationException MakeException(StringBuilder sb, Exception? inner = null)
    {
        sb.Append("\r\n Path: ");
        sb.AppendJoin("->", _errorTracePath.Select(x => "{" + x+ "}"));
        return new RouteConfigurationException(sb.ToString(), inner);
    }

    private static IReadOnlySet<Guid> MakeSet(List<RoutesGroup> groups)
    {
        var result = groups.Where(x => x.Id.HasValue).Select(x => x.Id!.Value).ToHashSet();
        if (result.Count == 0)
        {
            return _emptyIds; // Release heap object and use common empty set
        }

        return result;
    }

    private static readonly IReadOnlyList<RoutesGroup> _emptyGroups = new List<RoutesGroup>();
    private static readonly IReadOnlySet<Guid> _emptyIds = new HashSet<Guid>();
}
