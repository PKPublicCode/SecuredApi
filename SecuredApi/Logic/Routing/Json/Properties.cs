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
namespace SecuredApi.Logic.Routing.Json;

public static class Properties
{
    public const string RoutesGroupIdPropertyName = "id";
    public const string RoutesGroupsPropertyName = "routesGroups";
    public const string RoutesGroupPreRequestActions = "preRequestActions";
    public const string RoutesGroupOnErrorActions = "onRequestErrorActions";
    public const string RoutesGroupOnSuccessActions = "onRequestSuccessActions";

    public const string ActionsPropertyName = "actions";
    public const string ActionTypePropertyName = "type";
    public const string ActionSettingsPropertyName = "settings";

    public const string RouteIdPropertyName = "id";
    public const string RoutesPropertyName = "routes";
    public const string RouteKeyPropertyName = "routeKey";

    public const string NotFoundRouteActionsPropertyName = "routeNotFoundActions";
}
