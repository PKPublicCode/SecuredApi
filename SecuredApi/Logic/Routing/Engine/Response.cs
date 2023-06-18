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
using Microsoft.AspNetCore.Http;

namespace SecuredApi.Logic.Routing.Engine
{
    public class Response : IResponse
    {
        private readonly HttpResponse _response;
        public IHeaderDictionary Headers => _response.Headers;
        public IResponseStream? Body { get; set; }
        public int StatusCode
        {
            get => _response.StatusCode;
            set => _response.StatusCode = value;
        }
        public string? ContentType
        {
            get => _response.ContentType;
            set => _response.ContentType = value;
        }

        public Response(HttpResponse response)
        {
            _response = response;
            Body = null;
        }

        public void Dispose() => Body?.Dispose();
    }
}
