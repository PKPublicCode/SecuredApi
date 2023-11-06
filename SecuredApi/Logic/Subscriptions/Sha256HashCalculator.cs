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
using Microsoft.Extensions.Options;
using System.Text;
using System.Security.Cryptography;

namespace SecuredApi.Logic.Subscriptions;

public class Sha256HashCalculator: IHashCalculator
{
    private readonly byte[] _salt;

    public Sha256HashCalculator(IOptions<SubscriptionsSecurityCfg> options)
    {
        _salt = Encoding.UTF8.GetBytes(options.Value.Salt);
    }

    public string CalculateHash(string key)
    {
        using var hasher = SHA256.Create();
        int keyLen = Encoding.UTF8.GetByteCount(key, 0, key.Length);
        byte[] bytesInput = new byte[keyLen + _salt.Length];
        Encoding.UTF8.GetBytes(key, 0, key.Length, bytesInput, 0);
        Array.Copy(_salt, 0, bytesInput, keyLen, _salt.Length);
        var byteResult = hasher.ComputeHash(bytesInput);
        return Convert.ToHexString(byteResult);
        // return Convert.ToBase64String(byteResult);
    }
}

