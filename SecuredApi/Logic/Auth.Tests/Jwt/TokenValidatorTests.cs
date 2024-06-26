﻿// Copyright (c) 2021 - present, Pavlo Kruglov.
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
using SecuredApi.Testing.Common.Jwt;
using static SecuredApi.Testing.Common.Jwt.SigningKeys;
using static SecuredApi.Testing.Common.Jwt.TokenHelper;

namespace SecuredApi.Logic.Auth.Jwt;

public class TokenValidatorTests
{
    private ISigningKeysProvider _keysProvider = Substitute.For<ISigningKeysProvider>();
    private const string _appId = "FakeAppId";
    
    static TokenValidatorTests()
    {
        Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
    }

    [Fact]
    public async Task ValidateToken_WithNoRolesNoScope_StatusOk()
    {
        var allowedKeys = MakePublicKeysList(TestKey1, TestKey2);
        const string issuer = "https://my-issuer.com";
        const string audience = "api://my-audience";
        var token = CreateJwtToken(issuer, audience, TestKey2, Array.Empty<string>(), DateTime.UtcNow, TimeSpan.FromHours(1));
        SetAllowedKeys(issuer, allowedKeys);

        var result = await TokenValidator.ValidateTokenAsync(token, _keysProvider, new[] { issuer }, new[] { audience }, null, null);

        result.Status.Should().Be(ValidationStatus.Ok);
        result.Succeed.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateToken_WithValidRole_StatusOk()
    {
        var allowedKeys = MakePublicKeysList(TestKey1, TestKey2);
        const string issuer = "https://my-issuer.com";
        const string audience = "api://my-audience";
        var token = CreateJwtToken(issuer, audience, TestKey2, Array.Empty<string>(), DateTime.UtcNow, TimeSpan.FromHours(1), "Read Write");
        SetAllowedKeys(issuer, allowedKeys);
        
        var result = await TokenValidator.ValidateTokenAsync(token, _keysProvider, new[] { issuer }, new[] { audience }, null, new[] { "All", "Read" });

        result.Status.Should().Be(ValidationStatus.Ok);
    }

    [Fact]
    public async Task ValidateToken_WithValidScope_StatusOk()
    {
        var allowedKeys = MakePublicKeysList(TestKey1, TestKey2);
        const string issuer = "https://my-issuer.com";
        const string audience = "api://my-audience";
        var token = CreateJwtToken(issuer, audience, TestKey1, new[] { "Guest", "User" }, DateTime.UtcNow, TimeSpan.FromHours(1));
        SetAllowedKeys(issuer, allowedKeys);

        var result = await TokenValidator.ValidateTokenAsync(token, _keysProvider, new[] { issuer }, new[] { audience }, new[] { "User", "Admin" }, null!);

        result.Status.Should().Be(ValidationStatus.Ok);
    }

    [Fact]
    public async Task ValidateToken_WithoutRequiredRoles_AccessDenied()
    {
        var allowedKeys = MakePublicKeysList(TestKey1, TestKey2);
        const string issuer = "https://my-issuer.com";
        const string audience = "api://my-audience";
        var token = CreateJwtToken(issuer, audience, TestKey2, new[] { "Guest" }, DateTime.UtcNow, TimeSpan.FromHours(1));
        SetAllowedKeys(issuer, allowedKeys);

        var result = await TokenValidator.ValidateTokenAsync(token, _keysProvider, new[] { issuer }, new[] { audience }, new[] { "User", "Admin" }, null!);

        result.Status.Should().Be(ValidationStatus.AccessDenied);
    }

    [Fact]
    public async Task ValidateToken_WithoutRequiredScope_AccessDenied()
    {
        var allowedKeys = MakePublicKeysList(TestKey1, TestKey2);
        const string issuer = "https://my-issuer.com";
        const string audience = "api://my-audience";
        var token = CreateJwtToken(issuer, audience, TestKey2, Array.Empty<string>(), DateTime.UtcNow, TimeSpan.FromHours(1), "Other.Scope Other.Scope2");
        SetAllowedKeys(issuer, allowedKeys);

        var result = await TokenValidator.ValidateTokenAsync(token, _keysProvider, new[] { issuer }, new[] { audience }, null!, new[] { "Write", "Read" });

        result.Status.Should().Be(ValidationStatus.AccessDenied);
    }

    [Fact]
    public async Task ValidateToken_WithUnknownSigningKey_NotAuthorized()
    {
        var allowedKeys = MakePublicKeysList(TestKey1, TestKey2);
        const string issuer = "https://my-issuer.com";
        const string audience = "api://my-audience";
        var token = CreateJwtToken(issuer, audience, TestKey3, Array.Empty<string>(), DateTime.UtcNow, TimeSpan.FromHours(1));
        SetAllowedKeys(issuer, allowedKeys);

        var result = await TokenValidator.ValidateTokenAsync(token, _keysProvider, new[] { issuer }, new[] { audience }, EmptyStrings, EmptyStrings);

        result.Status.Should().Be(ValidationStatus.NotAuthorized);
    }

    [Fact]
    public async Task ValidateToken_TokenNotSigned_NotAuthorized()
    {
        var allowedKeys = MakePublicKeysList(TestKey1, TestKey2);
        const string issuer = "https://my-issuer.com";
        const string audience = "api://my-audience";
        var token = CreateJwtToken(issuer, audience, null, Array.Empty<string>(), DateTime.UtcNow, TimeSpan.FromHours(1));
        SetAllowedKeys(issuer, allowedKeys);

        var result = await TokenValidator.ValidateTokenAsync(token, _keysProvider, new[] { issuer }, new[] { audience }, EmptyStrings, EmptyStrings);

        result.Status.Should().Be(ValidationStatus.NotAuthorized);
    }

    [Fact]
    public async Task ValidateToken_TokenNotSignedAndNoAllowedKeysProvided_NotAuthorized()
    {
        //testcase actually tests underlying JsonWebToken behavior. But nice to know that.
        const string issuer = "https://my-issuer.com";
        const string audience = "api://my-audience";
        var token = CreateJwtToken(issuer, audience, null, Array.Empty<string>(), DateTime.UtcNow, TimeSpan.FromHours(1));
        SetAllowedKeys(issuer, Array.Empty<SecurityKey>());

        var result = await TokenValidator.ValidateTokenAsync(token, _keysProvider, new[] { issuer }, new[] { audience }, EmptyStrings, EmptyStrings);

        result.Status.Should().Be(ValidationStatus.NotAuthorized);
    }

    [Fact]
    public async Task ValidateToken_IssuerIsInvalid_AccessDenied()
    {
        var allowedKeys = MakePublicKeysList(TestKey1, TestKey2);
        const string issuer = "https://my-issuer.com";
        const string audience = "api://my-audience";
        var token = CreateJwtToken(issuer, audience, TestKey1, Array.Empty<string>(), DateTime.UtcNow, TimeSpan.FromHours(1));
        SetAllowedKeys(issuer, allowedKeys);

        var result = await TokenValidator.ValidateTokenAsync(token, _keysProvider, new[] { issuer + ".ua" }, new[] { audience }, EmptyStrings, EmptyStrings);

        result.Status.Should().Be(ValidationStatus.AccessDenied);
    }

    [Fact]
    public async Task ValidateToken_AudienceIsInvalid_AccessDenied()
    {
        var allowedKeys = MakePublicKeysList(TestKey1, TestKey2);
        const string issuer = "https://my-issuer.com";
        var token = CreateJwtToken(issuer, "api://my-audience", TestKey2, Array.Empty<string>(), DateTime.UtcNow, TimeSpan.FromHours(1));
        SetAllowedKeys(issuer, allowedKeys);

            var result = await TokenValidator.ValidateTokenAsync(token, _keysProvider, new[] { issuer }, new[] { "api://another-audience" }, EmptyStrings, EmptyStrings);

        result.Status.Should().Be(ValidationStatus.AccessDenied);
    }

    [Fact]
    public async Task ValidateToken_TokenIsExpired_NotAuthorized()
    {
        var allowedKeys = MakePublicKeysList(TestKey1, TestKey2);
        const string issuer = "https://my-issuer.com";
        const string audience = "api://my-audience";
        var token = CreateJwtToken(issuer, audience, TestKey1, Array.Empty<string>(), DateTime.UtcNow.AddHours(-1), TimeSpan.FromMinutes(1));
        SetAllowedKeys(issuer, allowedKeys);

        var result = await TokenValidator.ValidateTokenAsync(token, _keysProvider, new[] { issuer }, new[] { audience }, EmptyStrings, EmptyStrings);

        result.Status.Should().Be(ValidationStatus.NotAuthorized);
    }

    private void SetAllowedKeys(string issuer, IEnumerable<SecurityKey> keys)
        => _keysProvider.GetKeysAsync(issuer, Arg.Any<CancellationToken>()).Returns(keys);

    private static string[] EmptyStrings => Array.Empty<string>();

    private static string CreateJwtToken(string issuer,
                                        string audience,
                                        RsaKeyInfo? key,
                                        IEnumerable<string> roles,
                                        DateTime start,
                                        TimeSpan duration,
                                        string? scope = null)
    {
        return TokenHelper.CreateJwtToken(_appId, issuer, audience, key, roles, start, duration, scope);
    }
}
