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
namespace SecuredApi.Logic.Routing.Model.Actions.Auth;

/// <summary>
/// Runs actions configured for the specified consumer.
/// </summary>
/// <remarks>
/// Executes actions configured for the current consumer (client).
/// RunConsumerActions has to be exectuted after one of authentication actions,
/// that saves current consumer id.
/// </remarks>
/// <return>
/// Fails when:
///
/// * one of consumer actions fails. HTTP code in client response is set according to the consumer action
///
/// * if consumer id is invalid, not found, or CheckSubscription action wasn't executed for this rote. In this case 500 HTTP code is set to client response, indicating that data is corrupted
///
/// If consumer actions are successful (if any), the action succeeds .
/// </return>
public class RunConsumerActions
{
    /// <summary>
    /// Configures behavior when record for customer is not found:
    /// If false and there are no record, then call will fail with 500 Http Error.
    /// If true and there are no record, error will be ignored and execution pass to the next route action.
    /// </summary>
    /// <value>false</value>
    public bool IgnoreIfAbsent { get; init; } = false;
}

