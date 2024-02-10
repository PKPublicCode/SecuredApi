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
using System.Net.Mime;
using System.Text;

namespace SecuredApi.WebApps.Gateway.Utils;

public static class HttpRequestMessageExtensions
{
    public static HttpRequestMessage SetPost(this HttpRequestMessage msg)
    {
        msg.Method = HttpMethod.Post;
        return msg;
    }

    public static HttpRequestMessage SetRelativePath(this HttpRequestMessage msg, string path)
    {
        msg.RequestUri = new Uri(path, UriKind.Relative);
        return msg;
    }

    public static HttpRequestMessage SetStringContent(this HttpRequestMessage msg, string content)
    {
        msg.Content = new StringContent(content, Encoding.UTF8, MediaTypeNames.Text.Plain);
        return msg;
    }

    public static HttpRequestMessage AddHeader(this HttpRequestMessage msg, string name, string value)
    {
        msg.Headers.Add(name, value);
        return msg;
    }
}

