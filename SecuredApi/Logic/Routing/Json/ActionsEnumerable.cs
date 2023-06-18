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
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;

namespace SecuredApi.Logic.Routing.Json
{
    internal struct ActionsEnumerable: IEnumerable<IAction>
    {
        private JsonElement _json;
        private readonly ActionsEnumeratorConfig _config;

        public ActionsEnumerable(JsonElement json, ActionsEnumeratorConfig config)
        {
            _json = json;
            _config = config;
        }

        public ActionsEnumerator GetEnumerator() => new ActionsEnumerator(_json, _config);

        IEnumerator<IAction> IEnumerable<IAction>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
