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
namespace SecuredApi.Logic.Routing;

public record RouteRecord
(
    IRequestProcessor RequestProcessor, // Holds list of actions and process them
    IReadOnlyList<RoutesGroup> Groups, // List of all parent RoutesGroup, used to run PreRequest and onRequestXXX actions
    IReadOnlySet<Guid> GroupIds, // Ids of the groups, used for faster search by allowed ids
    Guid? RouteId = null // Id of specific route, not used right now. Can be used same as GroupIds
);
