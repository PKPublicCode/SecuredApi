// Copyright (c) 2021 - present, Pavlo Kruglov.
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

using Microsoft.Extensions.Primitives;
using System.Text;

namespace SecuredApi.Logic.Routing.RuntimeExpressions;

public class TransformQueryStringPart: IRuntimeExpression
{
    private readonly string _beginning;
    private readonly string _separator;
    private readonly List<(string OldName, string NewNameWithEquality)> _params;
    
    public TransformQueryStringPart(string parameters)
    {
        var args = parameters.Split(',');

        if (args.Length < 3)
        {
            throw new InvalidOperationException($"TransformQueryString function should have at least 3 parameters");
        }

        _beginning = Unwrap(args[0]);
        _separator = Unwrap(args[1]);
        string equality = Unwrap(args[2]);
        _params = new(args.Length - 3);

        for (int i = 3; i < args.Length; ++i)
        {
            var pair = Unwrap(args[i]).Split(':');
            if (pair.Length != 2)
            {
                throw new InvalidOperationException($"Parameters mapping in TransformQueryString function has to be in format OldName:NewName"); 
            }
            _params.Add((pair[0], pair[1] + equality));
        }

        _params = _params.OrderBy(x => x.NewNameWithEquality).ToList();
    }
    
    public string Evaluate(IRequestContext ctx)
    {
        var sb = new StringBuilder();
        sb.Append(_beginning);
        int total = 0;
        foreach (var p in _params)
        {
            if (ctx.Request.Query.TryGetValue(p.OldName, out var value)
                && value.Count > 0)
            {
                if (total > 0)
                {
                    sb.Append(_separator);
                }
                sb.Append(p.NewNameWithEquality);
                sb.Append(value[0]);
                ++total;
            }
        }

        return total == 0 ? string.Empty : sb.ToString();
    }
    
    private static string Unwrap(string p)
    {
        if (p[0] == _quote && p[^1] == _quote)
        {
            return p.Substring(1, p.Length - 2);
        }
        
        throw new InvalidOperationException($"Unable to parse argument of TransformQueryString function");
    }

    private const char _quote = '\'';
}