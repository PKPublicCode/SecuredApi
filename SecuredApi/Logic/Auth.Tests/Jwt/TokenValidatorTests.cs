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
using static SecuredApi.Logic.Auth.Jwt.SigningKeys;

namespace SecuredApi.Logic.Auth.Jwt;

public class TokenValidatorTests
{
    ISigningKeysProvider _keysProvider = Substitute.For<ISigningKeysProvider>();
    
    static TokenValidatorTests()
    {
        Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
    }

    [Fact]
    public void NoRolesNoScope_Valid()
    {
        var allowedKeys = MakePublicKeysList(TestKey1, TestKey2);
        const string issuer = "https://my-issuer.com";
        const string audience = "api://my-audience";
        var token = CreateJwtToken(issuer, audience, TestKey2, Array.Empty<string>(), DateTime.UtcNow, TimeSpan.FromHours(1));
        SetAllowedKeys(issuer, allowedKeys);

        var result = TokenValidator.ValidateToken(token, _keysProvider, issuer, MakeList(audience), null!, null!);

        result.Status.Should().Be(ValidationStatus.Ok);
        result.Succeed.Should().BeTrue();
    }

    [Fact]
    public void ValidRole_Valid()
    {
        var allowedKeys = MakePublicKeysList(TestKey1, TestKey2);
        const string issuer = "https://my-issuer.com";
        const string audience = "api://my-audience";
        var token = CreateJwtToken(issuer, audience, TestKey2, Array.Empty<string>(), DateTime.UtcNow, TimeSpan.FromHours(1), "Read Write");
        SetAllowedKeys(issuer, allowedKeys);
        
        var result = TokenValidator.ValidateToken(token, _keysProvider, issuer, MakeList(audience), null!, MakeList("All", "Read"));

        result.Status.Should().Be(ValidationStatus.Ok);
    }

    [Fact]
    public void ValidScope_Valid()
    {
        var allowedKeys = MakePublicKeysList(TestKey1, TestKey2);
        const string issuer = "https://my-issuer.com";
        const string audience = "api://my-audience";
        var token = CreateJwtToken(issuer, audience, TestKey2, MakeList("Guest", "User"), DateTime.UtcNow, TimeSpan.FromHours(1));
        SetAllowedKeys(issuer, allowedKeys);

        var result = TokenValidator.ValidateToken(token, _keysProvider, issuer, MakeList(audience), MakeList("User", "Admin"), null!);

        result.Status.Should().Be(ValidationStatus.Ok);
    }

    [Fact]
    public void NoRequiredRoles_AccessDenied()
    {
        var allowedKeys = MakePublicKeysList(TestKey1, TestKey2);
        const string issuer = "https://my-issuer.com";
        const string audience = "api://my-audience";
        var token = CreateJwtToken(issuer, audience, TestKey2, MakeList("Guest"), DateTime.UtcNow, TimeSpan.FromHours(1));
        SetAllowedKeys(issuer, allowedKeys);

        var result = TokenValidator.ValidateToken(token, _keysProvider, issuer, MakeList(audience), MakeList("User", "Admin"), null!);

        result.Status.Should().Be(ValidationStatus.AccessDenied);
    }

    [Fact]
    public void NoRequiredScope_Valid()
    {
        var allowedKeys = MakePublicKeysList(TestKey1, TestKey2);
        const string issuer = "https://my-issuer.com";
        const string audience = "api://my-audience";
        var token = CreateJwtToken(issuer, audience, TestKey2, Array.Empty<string>(), DateTime.UtcNow, TimeSpan.FromHours(1), "Other.Scope Other.Scope2");
        SetAllowedKeys(issuer, allowedKeys);

        var result = TokenValidator.ValidateToken(token, _keysProvider, issuer, MakeList(audience), null!, MakeList("Write", "Read"));

        result.Status.Should().Be(ValidationStatus.AccessDenied);
    }

    [Fact]
    public void SigningKeyIsInvalid_NotAuthorized()
    {
        var allowedKeys = MakePublicKeysList(TestKey1, TestKey2);
        const string issuer = "https://my-issuer.com";
        const string audience = "api://my-audience";
        var token = CreateJwtToken(issuer, audience, TestKey3, Array.Empty<string>(), DateTime.UtcNow, TimeSpan.FromHours(1));
        SetAllowedKeys(issuer, allowedKeys);

        var result = TokenValidator.ValidateToken(token, _keysProvider, issuer, MakeList(audience), EmptyStrings, EmptyStrings);

        result.Status.Should().Be(ValidationStatus.NotAuthorized);
    }

    [Fact]
    public void IssuerIsInvalid_AccessDenied()
    {
        var allowedKeys = MakePublicKeysList(TestKey1, TestKey2);
        const string issuer = "https://my-issuer.com";
        const string audience = "api://my-audience";
        var token = CreateJwtToken(issuer, audience, TestKey3, Array.Empty<string>(), DateTime.UtcNow, TimeSpan.FromHours(1));
        SetAllowedKeys(issuer, allowedKeys);

        var result = TokenValidator.ValidateToken(token, _keysProvider, issuer + ".ua", MakeList(audience), EmptyStrings, EmptyStrings);

        result.Status.Should().Be(ValidationStatus.AccessDenied);
    }

    [Fact]
    public void AudiencceIsInvalid_AccessDenied()
    {
        var allowedKeys = MakePublicKeysList(TestKey1, TestKey2);
        const string issuer = "https://my-issuer.com";
        var token = CreateJwtToken(issuer, "api://my-audience", TestKey3, Array.Empty<string>(), DateTime.UtcNow, TimeSpan.FromHours(1));
        SetAllowedKeys(issuer, allowedKeys);

        var result = TokenValidator.ValidateToken(token, _keysProvider, issuer + ".ua", MakeList("api://another-audience"), EmptyStrings, EmptyStrings);

        result.Status.Should().Be(ValidationStatus.AccessDenied);
    }

    [Fact]
    public void TokenIsExpired_MotAuthorized()
    {
        var allowedKeys = MakePublicKeysList(TestKey1, TestKey2);
        const string issuer = "https://my-issuer.com";
        const string audience = "api://my-audience";
        var token = CreateJwtToken(issuer, audience, TestKey1, Array.Empty<string>(), DateTime.UtcNow.AddHours(-1), TimeSpan.FromMinutes(1));
        SetAllowedKeys(issuer, allowedKeys);

        var result = TokenValidator.ValidateToken(token, _keysProvider, issuer, MakeList(audience), EmptyStrings, EmptyStrings);

        result.Status.Should().Be(ValidationStatus.NotAuthorized);
    }

    private void SetAllowedKeys(string issuer, IEnumerable<SecurityKey> keys)
        => _keysProvider.GetKeysAsync(issuer, Arg.Any<CancellationToken>()).Returns(keys);

    private static T[] MakeList<T>(params T[] a) => a;
    private static string[] EmptyStrings => Array.Empty<string>();

    private static string CreateJwtToken(string issuer, string audience, RsaKeyInfo key, IEnumerable<string> roles, DateTime start, TimeSpan duration, string? scope = null)
    {
        using var privateKey = RSA.Create();
        privateKey.ImportFromPem(key.Private);
        var signingKey = new RsaSecurityKey(privateKey) { KeyId = key.Kid };

        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.RsaSha256)
        {
            CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false },
        };

        var claims = new ClaimsIdentity(
                roles.Select(r => new Claim("roles", r))
            );
        if (scope != null)
        {
            claims.AddClaim(new Claim("scp", scope));
        }

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

    private static IEnumerable<SecurityKey> MakePublicKeysList(params RsaKeyInfo[] keys)
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

