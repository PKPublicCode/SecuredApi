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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SecuredApi.Logic.Routing.Engine.PartialRoutingTable
{
    //Trie-Tree implementation.
    //Implementation based on dictionary, that doesn't support ReadOnlySpan in index opertors.
    //As result a lot of substring allocations. Maybe custom dictionary should be implemented for
    //more allocation-efficient search.
    internal class RoutesTree: IRoutesTree
    {
        private readonly Node _root = new Node();

        public RoutesTree()
        {
        }

        public void AddRoute(string path, RouteRecord record)
        {
            var p = new Context()
            {
                Path = path
            };
            var curNode = _root;
            while(true)
            {
                Parse(ref p);
                switch(p.State)
                {
                    case State.RoutePart:
                        if (curNode.HasRoute && curNode.Wildcard)
                        {
                            throw new RouteConfigurationException($"Problem with routes config. Attempt to add route to wildcard node. {path}");
                        }
                        var routePart = p.RoutePart;
                        curNode.EnsureChildren();
                        if (curNode.Children!.TryGetValue(routePart, out var nextNode))
                        {
                            curNode = nextNode;
                        }
                        else
                        {
                            nextNode = new Node();
                            curNode.Children.Add(p.RoutePart, nextNode);
                            curNode = nextNode;
                        }
                        break;
                    case State.Wildcard:
                    case State.End:
                        if (curNode.HasRoute) //Already initialized as terminal node
                        {
                            throw new RouteConfigurationException($"Problem with routes config. Attempt to rewrite terminal node. {path}");
                        }
                        if (p.State == State.Wildcard)
                        {
                            curNode.SetWildcard(record);
                        }
                        else
                        {
                            curNode.SetRouteRecord(record);
                        }
                        return;
                    case State.ParamsStart:
                        throw new RouteConfigurationException($"Route can't have parameters. {path}");
                }
            }
        }

        public bool TryFindRote(string path, [MaybeNullWhen(false)] out RouteInfo info)
        {
            info = null;
            if (!_root.Initialized)
            {
                return false; //tree is empty
            }
            var p = new Context()
            {
                Path = path
            };
            Node curNode = _root;
            while(true)
            {
                Parse(ref p);
                if (p.State == State.Wildcard)
                {
                    return false; //Wildcards not allowed in path
                }
                if (p.State == State.RoutePart)
                {
                    if (curNode.HasChildren)
                    {
                        if (curNode.Children!.TryGetValue(p.RoutePart, out var nextNode))
                        {
                            curNode = nextNode;
                            continue;
                        }
                    }
                    if (!curNode.HasRoute || !curNode.Wildcard)
                    {
                        return false;
                    }
                    //At this point cur node has route and it's a wildcard
                }
                else
                {
                    //we get to the end of the path. So node should be a 'terminal' node
                    if (!curNode.HasRoute)
                    {
                        return false;
                    }
                }

                //there could be one of following cases:
                //1) route part + route has wildcard
                //2) end of path (parameters, or end of line) + route
                //In both cases return route that we've found
                info = new RouteInfo(curNode.RouteRecord!, p.RemainingPart);
                return true;
            }
        }

        private void Parse(ref Context p)
        {
            int begin = p.End;

            //Skip all slash symbols
            while (begin < p.Path.Length && p.Path[begin] == _routeDelimiter)
            {
                ++begin;
            }

            p.Begin = begin;
            p.End = begin;

            //Check if we get to the end of parsing
            if (begin == p.Path.Length)
            {
                p.State = State.End;
                return;
            }

            if (p.Path[begin] == _paramsDelimiter)
            {
                p.State = State.ParamsStart;
                return;
            }

            if (p.Path[begin] == _wildCard)
            {
                p.State = State.Wildcard;
                return;
            }

            int end = begin + 1;
            while(end != p.Path.Length
                    && p.Path[end] != _paramsDelimiter
                    && p.Path[end] != _wildCard //ToDo Consider set wildcard as error, if it not follows slash
                    && p.Path[end] != _routeDelimiter)
            {
                ++end;
            }
            p.End = end;
            p.State = State.RoutePart;
        }

        private const char _routeDelimiter = '/';
        private const char _paramsDelimiter = '?';
        private const char _wildCard = '*';

        private struct Context
        {
            public int Begin { get; set; }
            public int End { get; set; }
            public string Path { get; init; }
            public State State { get; set; }
            public string RoutePart => Path[Begin..End].ToLower();
            public string RemainingPart => Path[Begin..];
        }

        private enum State
        {
            Unknown,
            End,
            RoutePart,
            ParamsStart,
            Wildcard
        }

        private class Node
        {
            public Node() { }
            public Dictionary<string, Node>? Children { get; private set; }
            public RouteRecord? RouteRecord { get; private set; }

            public bool Wildcard { get; private set; }
            public bool HasRoute => RouteRecord != null;
            public bool HasChildren => Children != null;
            public bool Initialized => RouteRecord != null || Children != null;

            public void EnsureChildren() => Children ??= new ();
            public void SetWildcard(RouteRecord record)
            {
                RouteRecord = record;
                Wildcard = true;
            }
            public void SetRouteRecord(RouteRecord record) => RouteRecord = record;
        }
    }
}
