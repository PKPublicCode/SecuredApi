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
using static SecuredApi.Testing.Common.Jwt.SigningKeys;
using static SecuredApi.Testing.Common.Jwt.TokenHelper;

namespace SecuredApi.Logic.Auth.Jwt;

public class TokenValidatorTests
{
    ISigningKeysProvider _keysProvider = Substitute.For<ISigningKeysProvider>();
    
    static TokenValidatorTests()
    {
        Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
    }

    [Fact]
    public async Task NoRolesNoScope_Valid()
    {
        var allowedKeys = MakePublicKeysList(TestKey1, TestKey2);
        const string issuer = "https://my-issuer.com";
        const string audience = "api://my-audience";
        var token = CreateJwtToken(issuer, audience, TestKey2, Array.Empty<string>(), DateTime.UtcNow, TimeSpan.FromHours(1));
        SetAllowedKeys(issuer, allowedKeys);

        var result = await TokenValidator.ValidateTokenAsync(token, _keysProvider, MakeList(issuer), MakeList(audience), null!, null!);

        result.Status.Should().Be(ValidationStatus.Ok);
        result.Succeed.Should().BeTrue();
    }

    [Fact]
    public async Task ValidRole_Valid()
    {
        var allowedKeys = MakePublicKeysList(TestKey1, TestKey2);
        const string issuer = "https://my-issuer.com";
        const string audience = "api://my-audience";
        var token = CreateJwtToken(issuer, audience, TestKey2, Array.Empty<string>(), DateTime.UtcNow, TimeSpan.FromHours(1), "Read Write");
        SetAllowedKeys(issuer, allowedKeys);
        
        var result = await TokenValidator.ValidateTokenAsync(token, _keysProvider, MakeList(issuer), MakeList(audience), null!, MakeList("All", "Read"));

        result.Status.Should().Be(ValidationStatus.Ok);
    }

    [Fact]
    public async Task ValidScope_Valid()
    {
        var allowedKeys = MakePublicKeysList(TestKey1, TestKey2);
        const string issuer = "https://my-issuer.com";
        const string audience = "api://my-audience";
        var token = CreateJwtToken(issuer, audience, TestKey1, MakeList("Guest", "User"), DateTime.UtcNow, TimeSpan.FromHours(1));
        SetAllowedKeys(issuer, allowedKeys);

        var result = await TokenValidator.ValidateTokenAsync(token, _keysProvider, MakeList(issuer), MakeList(audience), MakeList("User", "Admin"), null!);

        result.Status.Should().Be(ValidationStatus.Ok);
    }

    [Fact]
    public async Task NoRequiredRoles_AccessDenied()
    {
        var allowedKeys = MakePublicKeysList(TestKey1, TestKey2);
        const string issuer = "https://my-issuer.com";
        const string audience = "api://my-audience";
        var token = CreateJwtToken(issuer, audience, TestKey2, MakeList("Guest"), DateTime.UtcNow, TimeSpan.FromHours(1));
        SetAllowedKeys(issuer, allowedKeys);

        var result = await TokenValidator.ValidateTokenAsync(token, _keysProvider, MakeList(issuer), MakeList(audience), MakeList("User", "Admin"), null!);

        result.Status.Should().Be(ValidationStatus.AccessDenied);
    }

    [Fact]
    public async Task NoRequiredScope_Valid()
    {
        var allowedKeys = MakePublicKeysList(TestKey1, TestKey2);
        const string issuer = "https://my-issuer.com";
        const string audience = "api://my-audience";
        var token = CreateJwtToken(issuer, audience, TestKey2, Array.Empty<string>(), DateTime.UtcNow, TimeSpan.FromHours(1), "Other.Scope Other.Scope2");
        SetAllowedKeys(issuer, allowedKeys);

        var result = await TokenValidator.ValidateTokenAsync(token, _keysProvider, MakeList(issuer), MakeList(audience), null!, MakeList("Write", "Read"));

        result.Status.Should().Be(ValidationStatus.AccessDenied);
    }

    [Fact]
    public async Task SigningKeyIsInvalid_NotAuthorized()
    {
        var allowedKeys = MakePublicKeysList(TestKey1, TestKey2);
        const string issuer = "https://my-issuer.com";
        const string audience = "api://my-audience";
        var token = CreateJwtToken(issuer, audience, TestKey3, Array.Empty<string>(), DateTime.UtcNow, TimeSpan.FromHours(1));
        SetAllowedKeys(issuer, allowedKeys);

        var result = await TokenValidator.ValidateTokenAsync(token, _keysProvider, MakeList(issuer), MakeList(audience), EmptyStrings, EmptyStrings);

        result.Status.Should().Be(ValidationStatus.NotAuthorized);
    }

    [Fact]
    public async Task TokenNotSigned_NotAuthorized()
    {
        var allowedKeys = MakePublicKeysList(TestKey1, TestKey2);
        const string issuer = "https://my-issuer.com";
        const string audience = "api://my-audience";
        var token = CreateJwtToken(issuer, audience, null, Array.Empty<string>(), DateTime.UtcNow, TimeSpan.FromHours(1));
        SetAllowedKeys(issuer, allowedKeys);

        var result = await TokenValidator.ValidateTokenAsync(token, _keysProvider, MakeList(issuer), MakeList(audience), EmptyStrings, EmptyStrings);

        result.Status.Should().Be(ValidationStatus.NotAuthorized);
    }

    [Fact]
    public async Task TokenNotSignedAndNoAllowedKeysProvided_NotAuthorized()
    {
        //testcase actually tests underlying JsonWebToken behavior. But nice to know that.
        const string issuer = "https://my-issuer.com";
        const string audience = "api://my-audience";
        var token = CreateJwtToken(issuer, audience, null, Array.Empty<string>(), DateTime.UtcNow, TimeSpan.FromHours(1));
        SetAllowedKeys(issuer, Array.Empty<SecurityKey>());

        var result = await TokenValidator.ValidateTokenAsync(token, _keysProvider, MakeList(issuer), MakeList(audience), EmptyStrings, EmptyStrings);

        result.Status.Should().Be(ValidationStatus.NotAuthorized);
    }

    [Fact]
    public async Task IssuerIsInvalid_AccessDenied()
    {
        var allowedKeys = MakePublicKeysList(TestKey1, TestKey2);
        const string issuer = "https://my-issuer.com";
        const string audience = "api://my-audience";
        var token = CreateJwtToken(issuer, audience, TestKey1, Array.Empty<string>(), DateTime.UtcNow, TimeSpan.FromHours(1));
        SetAllowedKeys(issuer, allowedKeys);

        var result = await TokenValidator.ValidateTokenAsync(token, _keysProvider, MakeList(issuer + ".ua"), MakeList(audience), EmptyStrings, EmptyStrings);

        result.Status.Should().Be(ValidationStatus.AccessDenied);
    }

    [Fact]
    public async Task AudienceIsInvalid_AccessDenied()
    {
        var allowedKeys = MakePublicKeysList(TestKey1, TestKey2);
        const string issuer = "https://my-issuer.com";
        var token = CreateJwtToken(issuer, "api://my-audience", TestKey2, Array.Empty<string>(), DateTime.UtcNow, TimeSpan.FromHours(1));
        SetAllowedKeys(issuer, allowedKeys);

        var result = await TokenValidator.ValidateTokenAsync(token, _keysProvider, MakeList(issuer), MakeList("api://another-audience"), EmptyStrings, EmptyStrings);

        result.Status.Should().Be(ValidationStatus.AccessDenied);
    }

    [Fact]
    public async Task TokenIsExpired_NotAuthorized()
    {
        var allowedKeys = MakePublicKeysList(TestKey1, TestKey2);
        const string issuer = "https://my-issuer.com";
        const string audience = "api://my-audience";
        var token = CreateJwtToken(issuer, audience, TestKey1, Array.Empty<string>(), DateTime.UtcNow.AddHours(-1), TimeSpan.FromMinutes(1));
        SetAllowedKeys(issuer, allowedKeys);

        var result = await TokenValidator.ValidateTokenAsync(token, _keysProvider, MakeList(issuer), MakeList(audience), EmptyStrings, EmptyStrings);

        result.Status.Should().Be(ValidationStatus.NotAuthorized);
    }

    private void SetAllowedKeys(string issuer, IEnumerable<SecurityKey> keys)
        => _keysProvider.GetKeysAsync(issuer, Arg.Any<CancellationToken>()).Returns(keys);

    private static T[] MakeList<T>(params T[] a) => a;
    private static string[] EmptyStrings => Array.Empty<string>();
}

