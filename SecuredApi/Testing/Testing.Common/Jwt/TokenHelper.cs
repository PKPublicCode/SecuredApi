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
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace SecuredApi.Testing.Common.Jwt;

public static class TokenHelper
{
    public static string CreateJwtToken(string appId,
                                        string issuer,
                                        string audience,
                                        RsaKeyInfo? key,
                                        IEnumerable<string> roles,
                                        DateTime start,
                                        TimeSpan duration,
                                        string? scope = null)
    {
        using var privateKey = RSA.Create(); //will need it not disposable till the end
        SigningCredentials? signingCredentials = null;

        if (key != null)
        {
            privateKey.ImportFromPem(key.Private);
            var signingKey = new RsaSecurityKey(privateKey) { KeyId = key.Kid };
            signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.RsaSha256)
            {
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false },
            };
        }

        var claims = new ClaimsIdentity(
                roles.Select(r => new Claim("roles", r))
            );

        if (scope != null)
        {
            claims.AddClaim(new Claim("scp", scope));
        }

        claims.AddClaim(new Claim("appId", appId));

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            IssuedAt = start,
            NotBefore = start,
            Expires = start.Add(duration),
            Subject = claims,
            SigningCredentials = signingCredentials
        };

        var handler = new JwtSecurityTokenHandler();

        var token = handler.CreateJwtSecurityToken(descriptor);

        return handler.WriteToken(token);
    }

    public static IEnumerable<SecurityKey> MakePublicKeysList(params RsaKeyInfo[] keys)
    {
        return keys.Select(x =>
        {
            var rsa = RSA.Create();
            rsa.ImportFromPem(x.Public);
            return new RsaSecurityKey(rsa) { KeyId = x.Kid };
            //var jwk = JsonWebKeyConverter.ConvertFromSecurityKey(new RsaSecurityKey(rsa) { KeyId = x.Kid });
            //return jwk;
        }).ToArray();
    }
}

