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
using System.Collections;
using System.Text.Json;
using static SecuredApi.Logic.Routing.Json.Properties;

namespace SecuredApi.Logic.Routing.Json;

internal struct ActionsEnumerator : IEnumerator<IAction>
{
    private JsonElement.ArrayEnumerator _enumerator;
    private IAction? _current;
    private readonly ActionsEnumeratorConfig _config;

    public ActionsEnumerator(JsonElement json, ActionsEnumeratorConfig config)
    {
        _enumerator = json.EnumerateArray();
        _config = config;
        _current = null;
    }

    public IAction Current
    {
        get => _current ?? throw new InvalidOperationException("Incorrect enumerator state");
        private set => _current = value;
    }

    object IEnumerator.Current => Current;

    public void Dispose()
    {
        _enumerator.Dispose();
    }

    public bool MoveNext()
    {
        if (_enumerator.MoveNext())
        {
            var actionJson = _enumerator.Current;
            if (!actionJson.TryGetProperty(ActionTypePropertyName, out var prop))
            {
                throw new RouteConfigurationException("Action name not set");
            }
            string type = prop.GetString() 
                ?? throw new RouteConfigurationException("Action name is null");
            var settingsType = _config.ActionFactory.GetSettingsType(type);
            if (!actionJson.TryGetProperty(ActionSettingsPropertyName, out prop))
            {
                throw new RouteConfigurationException("Settings not configured");
            }
            var settings = JsonSerializer.Deserialize(prop.GetRawText(), settingsType, _config.SerializerOptions)
                ?? throw new RouteConfigurationException("Invalid action settings");
            _current = _config.ActionFactory.CreateAction(type, settings);
            return true;
        }
        return false;
    }

    public void Reset()
    {
        _enumerator.Reset();
        Current = null!;
    }
}
