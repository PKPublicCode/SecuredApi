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
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using SecuredApi.Logic.Routing.Actions.OAuth;
using SecuredApi.Logic.Routing.Engine;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace SecuredApi.ComponentTests.Gateway;

public class OAuthTests: GatewayTestsBase
{
    private const string _keysJson = "{\"keys\":[{\"kty\":\"RSA\",\"use\":\"sig\",\"kid\":\"T1St-dLTvyWRgxB_676u8krXS-I\",\"x5t\":\"T1St-dLTvyWRgxB_676u8krXS-I\",\"n\":\"s2TCRTB0HKEfLBPi3_8CxCbWirz7rlvzcXnp_0j3jrmb_hst0iiHifSBwE0FV1WW79Kyw0AATkLfSLLyllyCuzgoUOgmXd3YMaqB8mQOBIecFQDAHkM1syzi_VwVdJt8H1yI0hOGcOktujDPHidVFtOuoDqAWlCs7kCGwlazK4Sfu_pnfJI4RmU8AvqO0auGcxg24ICbpP01G0PgbvW8uhWSWSSTXmfdIh567JOHsgvFr0m1AUQv7wbeRxgyiHwn29h6g1bwSYJB4I6TMG-cDygvU9lNWFzeYhtqG4Z_cA3khWIMmTq3dVzCsi4iU309-c0FopWacTHouHyMRcpJFQ\",\"e\":\"AQAB\",\"x5c\":[\"MIIC/TCCAeWgAwIBAgIIUd7j/OIahkYwDQYJKoZIhvcNAQELBQAwLTErMCkGA1UEAxMiYWNjb3VudHMuYWNjZXNzY29udHJvbC53aW5kb3dzLm5ldDAeFw0yMzExMDExNjAzMjdaFw0yODExMDExNjAzMjdaMC0xKzApBgNVBAMTImFjY291bnRzLmFjY2Vzc2NvbnRyb2wud2luZG93cy5uZXQwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQCzZMJFMHQcoR8sE+Lf/wLEJtaKvPuuW/Nxeen/SPeOuZv+Gy3SKIeJ9IHATQVXVZbv0rLDQABOQt9IsvKWXIK7OChQ6CZd3dgxqoHyZA4Eh5wVAMAeQzWzLOL9XBV0m3wfXIjSE4Zw6S26MM8eJ1UW066gOoBaUKzuQIbCVrMrhJ+7+md8kjhGZTwC+o7Rq4ZzGDbggJuk/TUbQ+Bu9by6FZJZJJNeZ90iHnrsk4eyC8WvSbUBRC/vBt5HGDKIfCfb2HqDVvBJgkHgjpMwb5wPKC9T2U1YXN5iG2obhn9wDeSFYgyZOrd1XMKyLiJTfT35zQWilZpxMei4fIxFykkVAgMBAAGjITAfMB0GA1UdDgQWBBRNcCE3HDX+HOJOu/bKfLYoSX3/0jANBgkqhkiG9w0BAQsFAAOCAQEAExns169MDr1dDNELYNK0JDjPUA6GR50jqfc+xa2KOljeXErOdihSvKgDS/vnDN6fjNNZuOMDyr6jjLvRsT0jVWzf/B6v92FrPRa/rv3urGXvW5am3BZyVPipirbiolMTuork95G7y7imftK7117uHcMq3D8f4fxscDiDXgjEEZqjkuzYDGLaVWGJqpv5xE4w+K4o2uDwmEIeIX+rI1MEVucS2vsvraOrjqjHwc3KrzuVRSsOU7YVHyUhku+7oOrB4tYrVbYYgwd6zXnkdouVPqOX9wTkc9iTmbDP+rfkhdadLxU+hmMyMuCJKgkZbWKFES7ce23jfTMbpqoHB4pgtQ==\"],\"issuer\":\"https://login.microsoftonline.com/a9e2b040-93ef-4252-992e-0d9830029ae8/v2.0\"},{\"kty\":\"RSA\",\"use\":\"sig\",\"kid\":\"5B3nRxtQ7ji8eNDc3Fy05Kf97ZE\",\"x5t\":\"5B3nRxtQ7ji8eNDc3Fy05Kf97ZE\",\"n\":\"37fxbBQ8eAP7znqk8B8kUwVFEdV7N8WXSflojHJf9tNCyqrMd4gsAu4RUzhwlBlAHwYLhmwMNgRs-B4gLsEUaWJUjx5O4NxcfokC-6TL1p_IoLMGeqwFOMSBtxa1OnnL3eAQD1D7O0pOjJBst1_SZPswhdVzTEGoWod_1vMFDu02d3ogP_tuv2zl5Jd92t17Yuqb61wDKLzHoCeUMTVEzQS44n0mXrWJniaY0-_zs8opwwmUHRW3JJ_m0i5B1m9lFmcVUUKh5VYZPaec5ddCO47J91nZi0tcgADhqPzhAJF20cBEvkCP9kSJiiv2ssedlEbTSZGQTuC7OlP9G8tvvQ\",\"e\":\"AQAB\",\"x5c\":[\"MIIC/TCCAeWgAwIBAgIISlx9oAuA2/MwDQYJKoZIhvcNAQELBQAwLTErMCkGA1UEAxMiYWNjb3VudHMuYWNjZXNzY29udHJvbC53aW5kb3dzLm5ldDAeFw0yMzEyMDUxNzE2NTdaFw0yODEyMDUxNzE2NTdaMC0xKzApBgNVBAMTImFjY291bnRzLmFjY2Vzc2NvbnRyb2wud2luZG93cy5uZXQwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQDft/FsFDx4A/vOeqTwHyRTBUUR1Xs3xZdJ+WiMcl/200LKqsx3iCwC7hFTOHCUGUAfBguGbAw2BGz4HiAuwRRpYlSPHk7g3Fx+iQL7pMvWn8igswZ6rAU4xIG3FrU6ecvd4BAPUPs7Sk6MkGy3X9Jk+zCF1XNMQahah3/W8wUO7TZ3eiA/+26/bOXkl33a3Xti6pvrXAMovMegJ5QxNUTNBLjifSZetYmeJpjT7/OzyinDCZQdFbckn+bSLkHWb2UWZxVRQqHlVhk9p5zl10I7jsn3WdmLS1yAAOGo/OEAkXbRwES+QI/2RImKK/ayx52URtNJkZBO4Ls6U/0by2+9AgMBAAGjITAfMB0GA1UdDgQWBBR6Y4Oi5GGItIomQ0yZfH/woCAogzANBgkqhkiG9w0BAQsFAAOCAQEAaNbWUtHv3+ryZecDc7m6V1V1rWrVkUwC2QO78a2TprEN3owOeP0IHP42fbd/wcSsufTTtkk/J+fqL5dtsQ6zk2kDQfY5CgOyVCsaxVqHsg3t8fAWBkHiNScjZvRhLx4ll9QMOtLAwL4Os3Of0qtvP61zONP9sCJoUB6hkB33SRma1OyPZnYK/l3r0Y49+Ov0wahcdI4yZI72hFXlyyLnOT8dMbJDwZ9LNXA/BauEff4qTI4nIQk/lQKS6BjHzvXZbkHYEV/6M7r1g1syeahDmnaII+ZiBwp6tmAZKZC0Q0O7y3DmcPrHiZdv35AHadZY5cGWy1rw8NIMkaHWZ0mP6Q==\"],\"issuer\":\"https://login.microsoftonline.com/a9e2b040-93ef-4252-992e-0d9830029ae8/v2.0\"},{\"kty\":\"RSA\",\"use\":\"sig\",\"kid\":\"fwNs8F_h9KrHMA_OcTP1pGFWwyc\",\"x5t\":\"fwNs8F_h9KrHMA_OcTP1pGFWwyc\",\"n\":\"6Jiu4AU4ZWHBFEbO1-41P6dxKgGx7J31i5wNzH5eoJlsNjWrWoGlZip8ey_ZppcNMY0GY330p8YwdazqRX24mPkyOxbYF1uGEGB_XtmMOtuG45WyPlbARQl8hok7y_hbydS8uyfm_ZQXN7MLgju0f4_cYo-dgic5OaR3W6CWfgOrNnf287ZZ2HtJ8DZNm-oHE2_Tg9FFTIIkpltNIZ4rJ0uwzuy7zkep_Pfxptzmpd0rwd0F87IneYu-jtKUvHVVPJQ7yQvgin0rZR8tXIp_IzComGipktu_AJ89z3atOEt0_vZPizQIMRpToHjUTNXuXaDWIvCIJYMkvvl0HJxf1Q\",\"e\":\"AQAB\",\"x5c\":[\"MIIC6TCCAdGgAwIBAgIIV6K/4n2M5VAwDQYJKoZIhvcNAQELBQAwIzEhMB8GA1UEAxMYbG9naW4ubWljcm9zb2Z0b25saW5lLnVzMB4XDTIzMTEzMDAwMTAxNVoXDTI4MTEzMDAwMTAxNVowIzEhMB8GA1UEAxMYbG9naW4ubWljcm9zb2Z0b25saW5lLnVzMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA6Jiu4AU4ZWHBFEbO1+41P6dxKgGx7J31i5wNzH5eoJlsNjWrWoGlZip8ey/ZppcNMY0GY330p8YwdazqRX24mPkyOxbYF1uGEGB/XtmMOtuG45WyPlbARQl8hok7y/hbydS8uyfm/ZQXN7MLgju0f4/cYo+dgic5OaR3W6CWfgOrNnf287ZZ2HtJ8DZNm+oHE2/Tg9FFTIIkpltNIZ4rJ0uwzuy7zkep/Pfxptzmpd0rwd0F87IneYu+jtKUvHVVPJQ7yQvgin0rZR8tXIp/IzComGipktu/AJ89z3atOEt0/vZPizQIMRpToHjUTNXuXaDWIvCIJYMkvvl0HJxf1QIDAQABoyEwHzAdBgNVHQ4EFgQUTtiYd3S6DOacXBmYsKyr1EK67f4wDQYJKoZIhvcNAQELBQADggEBAGbSzomDLsU7BX6Vohf/VweoJ9TgYs4cYcdFwMrRQVMpMGKYU6HT7f8mDzRGqpursuJTsN9yIOk7s5xp+N7EL6XKauzo+VHOGJT1qbwzJXT1XY6DuzBrlhtY9C7AUHlpYAD4uWyt+JfuB+z5Qq5cbGr0dMS/EEKR/m0iBboUNDji6u9sUzGqUYn4tpBoE+y0J8UttankG/09PPHwQIjMxMXcBDmGi5VTp9eY5RFk9GQ4qdQJUp2hhdQZDVpz6lcPxhG92RPO/ca3P/9dvfI5aNaiSyV7vuK2NGCVGCTeo/okA+V5dm5jeuf0bupNEPnXSGyM8EHjcRjR+cHsby5pIGs=\"],\"issuer\":\"https://login.microsoftonline.com/a9e2b040-93ef-4252-992e-0d9830029ae8/v2.0\"}]}";

    public OAuthTests()
        :base(_defaultFileName, (srv, config) =>
        {
            var keyProvider = Substitute.For<ISigningKeysProvider>();

            keyProvider.GetKeysAsync(Arg.Any<CancellationToken>())
                   .Returns(x =>
                   {
                       var keys = JsonWebKeySet.Create(_keysJson);
                       return keys.Keys;
                   }
               );

            srv.AddSingleton<ISigningKeysProvider>(keyProvider);
        })
    {
    }

    [Fact] //ToDo.0 Move out of component tests
    public async Task TestAction()
    {
        var settings = new CheckEntraTokenActionSettings(
            Issuer: "https://sts.windows.net/a9e2b040-93ef-4252-992e-0d9830029ae8/",
            Audience: "api://securedapi-gateway-ptst"
            );
        var sut = new CheckEntraTokenAction(settings);
        var tokenStr = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IjVCM25SeHRRN2ppOGVORGMzRnkwNUtmOTdaRSIsImtpZCI6IjVCM25SeHRRN2ppOGVORGMzRnkwNUtmOTdaRSJ9.eyJhdWQiOiJhcGk6Ly9zZWN1cmVkYXBpLWdhdGV3YXktcHRzdCIsImlzcyI6Imh0dHBzOi8vc3RzLndpbmRvd3MubmV0L2E5ZTJiMDQwLTkzZWYtNDI1Mi05OTJlLTBkOTgzMDAyOWFlOC8iLCJpYXQiOjE3MDQ2NDkyMDIsIm5iZiI6MTcwNDY0OTIwMiwiZXhwIjoxNzA0NjUzMTAyLCJhaW8iOiJFMlZnWUlnNG1QTG1WWDdlcTJ1Ly9LdzB4Vk5xQUE9PSIsImFwcGlkIjoiMDg1NWE1MzAtOWYxZC00OTljLTliYjAtZGVjM2M5ZjU5NjllIiwiYXBwaWRhY3IiOiIxIiwiaWRwIjoiaHR0cHM6Ly9zdHMud2luZG93cy5uZXQvYTllMmIwNDAtOTNlZi00MjUyLTk5MmUtMGQ5ODMwMDI5YWU4LyIsIm9pZCI6ImFjN2M5MjMzLWI0YTItNDU0Yi05MTEwLWNlYWU3OGM1NmQ4YyIsInJoIjoiMC5BVWNBUUxEaXFlLVRVa0taTGcyWU1BS2E2Si04Rlc0WVpJSkV2RFVBYm1CeUZZOUhBQUEuIiwicm9sZXMiOlsiRWNob1Nydi5SZWFkLkFsbCJdLCJzdWIiOiJhYzdjOTIzMy1iNGEyLTQ1NGItOTExMC1jZWFlNzhjNTZkOGMiLCJ0aWQiOiJhOWUyYjA0MC05M2VmLTQyNTItOTkyZS0wZDk4MzAwMjlhZTgiLCJ1dGkiOiJHZ3NUd3ZmRG8wcTNLZDlFY1ltZUFnIiwidmVyIjoiMS4wIn0.OzTnbMT0RpudblK2tEcEufmEEhzhLAitd2dG8ITB3RdtWD1RciNbYy6ocp6CqbP-_TdWDTi0w534R5Nx9NivhxxTR1uiZWcPkXIMN_A31dq0nijml4PVkAaFljaMs1uEMLN_H56F1Y556uI_4ICddp6AuDql0kCiJ-ThY8Vb75sCxu8fHzNy08oH1Ulltw6bHsSv3QwiG07o6UxcJWXHjpj3Hw14anO9IzNecjJC14UXqggrEZibq5g5Qspl7yWB7u8bm3ahTBqPo5fZ0YPW5NR0_p8F9MXt8m0_ebCJ_s0_0TQjDrsIbCC5lTAi_Z7z8xHfXuYBbgMH0JnP9hnTTg";

        Request.Headers.Add(new("Authorization", "Bearer " + tokenStr));

        using var scope = _serviceProvider.CreateAsyncScope();
        Context.RequestServices = scope.ServiceProvider;
        var ctx = new RequestContext(null!, Context);

        bool result = await sut.ExecuteAsync(ctx);

        Assert.True(true);
    }

    [Fact]
    public void TokenTest()
    {
        var token = CreateRsaToken("https://test-authentication-service",
                                    "api://test-secure-gateway",
                                    new [] {
                                        "EchoSrv.Read.All",
                                        "EchoSrv.Write.All"
                                        },
                                    DateTime.UtcNow,
                                    TimeSpan.FromHours(1)
                                    );

        using var privateKey = RSA.Create();
        privateKey.ImportFromPem(_testKey.Public);
        var jwt = JsonWebKeyConverter.ConvertFromSecurityKey(new RsaSecurityKey(privateKey) { KeyId = _testKey.Kid });

        Assert.True(true);
    }

    string CreateRsaToken(string issuer, string audience, IEnumerable<string> roles, DateTime start, TimeSpan duration)
    {
        using var privateKey = RSA.Create();
        privateKey.ImportFromPem(_testKey.Private);

        var signingCredentials = new SigningCredentials(new RsaSecurityKey(privateKey) { KeyId = _testKey.Kid }, SecurityAlgorithms.RsaSha256)
        {
            CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false },
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            IssuedAt = start,
            NotBefore = start,
            Expires = start.Add(duration),
            Subject = new ClaimsIdentity(
                roles.Select(r => new Claim("Roles", r))
            ),
            SigningCredentials = signingCredentials
        };

        var handler = new JwtSecurityTokenHandler();

        var token = handler.CreateJwtSecurityToken(descriptor);

        return handler.WriteToken(token);
    }

    private RsaKeyInfo _testKey = new()
    {
        Kid = "MyTestKid",
        Public = @"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA1LB6ddBsWApATHxI+zGu
3VH/ibnAu8OMhXKQcap51z/QacUMunz7hrmlvgeUWP4QZWl9zajmbzBiNiwVmdkh
TNe4hKimA7cffFkXGRNqalDttS/n8gpuaUxKKh+vpLO+UaBPbiAnb/Jd6Pe/XyeX
acgUyhzD9V8WPpKP4qpl0HXAX4fDtU/2vG0hx7c2JNQC7AdqMJX4x3QRsUzIQhkp
kFhmqUas9z1hRAymLAeOr9RU0E0f/28OQ7UuIcWm1o7q5vztz5RmDB6+C0KExca3
bTM3IklG/y02UXa1a3AiMm+00KZiKf3ywHKzddDg/8z+scziD9mhjRhBT5zRs+2Q
OQIDAQAB
-----END PUBLIC KEY-----",

        Private = @"-----BEGIN RSA PRIVATE KEY-----
MIIEowIBAAKCAQEA1LB6ddBsWApATHxI+zGu3VH/ibnAu8OMhXKQcap51z/QacUM
unz7hrmlvgeUWP4QZWl9zajmbzBiNiwVmdkhTNe4hKimA7cffFkXGRNqalDttS/n
8gpuaUxKKh+vpLO+UaBPbiAnb/Jd6Pe/XyeXacgUyhzD9V8WPpKP4qpl0HXAX4fD
tU/2vG0hx7c2JNQC7AdqMJX4x3QRsUzIQhkpkFhmqUas9z1hRAymLAeOr9RU0E0f
/28OQ7UuIcWm1o7q5vztz5RmDB6+C0KExca3bTM3IklG/y02UXa1a3AiMm+00KZi
Kf3ywHKzddDg/8z+scziD9mhjRhBT5zRs+2QOQIDAQABAoIBAQCgxhaL4FVF59na
90gjuda5LjbAYU2zoYojhgpyIa+ganicu3t3rOplWQhUsV4ON18liayzPa2S9zwG
PyTE+0EU2Sx9+w4jWNXQJXg0WFzkqPBHOkNWz1PO/QBC/1jCY6zF/yyLznqBQPCE
HmLnBwKJz9kHPt9SJ+Kkwh1J9gyomjla6N6WVK22es6YJxV3GCZVGaIBwWUcNgce
zKKLe4VTe0xJBvezNvKru40V93igISQ3y4k9nnVZP3BoA6zeMJbiNhyyHKjdfJLm
vcFO+udYcaQEcywqhQWObX2fTd9c2LfJ8yeCIIJCiAso5S7ycMewlqtnv+H+thhB
qtI2F3IBAoGBAOtjZqQ6VsAxX56Cwk69vSoRb8g6+DRl4+3y3XMNb9TMNyc/NTAN
z7qj/6DRtlXhTK/euLfJ9/buPWAsP0+5mRfk+CPikem86ne3c5pc9vKiS2QYzIJl
1gjGOu6HYu2hkggMiyrFTQlmuaA6XITIiQ4c9LdzigWpRnPjHZuTwrbRAoGBAOdQ
PtbEUKXLWuAqnTu/EuRHLEF5aY9lKgMEzCKK4v8nzFX8BjLBtQ56mw/jAZZHgNJE
vLAX/bilNZwwKmUTltGRMh/EkmWFEC6/CqxAHGbUU3jiN98Bt/8fHN/wX6UUiNIP
2OYh3USIEwCVOSljz54kSHz3/KFAMa0OOZ2YhWzpAoGALEvXa/5ihuaDtQOsZz8D
kyAW5hpazRmDjCrRC66ypdwMYQFfE/z1Y40rNOtiIcU1Nj92iXejhz+MI0YQYANw
UPPQ0of3p4Haqc7HHXxzKHPsNhkIm11oqtwLCQpHTqrCHWum4NSiS6ueMQ3qjT5j
tFk0oDVI+wnA7VwHHVjwIpECgYBoLGiQeotDj6jWqfpz7OKKMk+JES/sJ4hbIa75
o4kFlpvc4Yq9EyYCZk8tQXP2hS9MAy7jM3rNzIGvXLXLHZ5ftT9YtUOlOt8F3n3l
A16HJPqOx3qYEYMW/6EWbX/1raDM0dxCEGBBO/Mq4QmETXI0a2zF8z0wNePpZy2l
fwof+QKBgHsr4ChoJneGKumr55nlRUM3iORyH6EaIsbLHlJH+MucP5UJJXLIpiaf
YUmv2qv9U1BFbLClDl5ouG+lVZ7xdd7Ym0b2q7ZdTkFcow42puPwm1Qq3/Y5YOTi
mSsKpzd+9lw9mJsHJbxuSa/vvLa99uS4xdAI33kLi861pOL2snoP
-----END RSA PRIVATE KEY-----"
    };

    private class RsaKeyInfo
    {
        public string Kid { get; init; } = string.Empty;
        public string Public { get; init; } = string.Empty;
        public string Private { get; init; } = string.Empty;
    }
}

