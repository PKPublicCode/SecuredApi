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
namespace SecuredApi.Logic.Auth.Jwt;

public static class SigningKeys
{
    public static readonly RsaKeyInfo TestKey1 = new()
    {
        Kid = "TestKey1Kid",
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

    public static readonly RsaKeyInfo TestKey2 = new()
    {
        Kid = "TestKey2Kid",
        Public = @"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA99ojR9Uu23XuNM4ANNuS
STfsXlhv3KjUvlYXcZ8pvxVCtpZsDUsIq4vJR+x3oEgbG2JP3uw5H6NYh7L8Z5G9
yZ4fCrULaLUC5SJ4TvXpKig/eGmH1mFtyBA1aAvQYljmcQwhwnhMtmCrQWKABql0
fUxDU3zDPWoDhI+6yMoc8Kzhf3MmYLnGNvoC9IdMCz1ErJ51p1+xqrqTlXzGSkRF
MXlNyMtlSWyqTPD8amzox1cwNPEYLNoIZsNGSMJvOooMgyEi9nXcyB9piDZDKiH1
gwgqK3URx5TLnU3T2U2z25SibeYQKX6AUGUmrkwpnEtrsVh9+c0U4Nya4bTgRAVb
iQIDAQAB
-----END PUBLIC KEY-----",

        Private = @"-----BEGIN RSA PRIVATE KEY-----
MIIEowIBAAKCAQEA99ojR9Uu23XuNM4ANNuSSTfsXlhv3KjUvlYXcZ8pvxVCtpZs
DUsIq4vJR+x3oEgbG2JP3uw5H6NYh7L8Z5G9yZ4fCrULaLUC5SJ4TvXpKig/eGmH
1mFtyBA1aAvQYljmcQwhwnhMtmCrQWKABql0fUxDU3zDPWoDhI+6yMoc8Kzhf3Mm
YLnGNvoC9IdMCz1ErJ51p1+xqrqTlXzGSkRFMXlNyMtlSWyqTPD8amzox1cwNPEY
LNoIZsNGSMJvOooMgyEi9nXcyB9piDZDKiH1gwgqK3URx5TLnU3T2U2z25SibeYQ
KX6AUGUmrkwpnEtrsVh9+c0U4Nya4bTgRAVbiQIDAQABAoIBAQCBL0q6Uyx0KPi6
vzCuXE/YJ/Tki/XqLnmMp7TafRo2Ra+bGbZCC3z+ZQLJoUAM4+h1En4M9FUl327y
c+FJXlZssyT84LaMoeRbZ+cLSPN6DLZgXaj2nZup7oDXtWitCic6fxdNlIRvft1k
XCHc1xAnisuAxYPCC/K/cmbp5ybEbmEYMqBYGEJ+xz2sNf+AbH8F1TYh5ybyPAyL
yf8pTWLCkEoQvsaxe30dxaU/ZUionA9PB/VpWusUHQoQ/IrCo5b5IfaZ7WK6ZIU3
7/taUUwwcepf/AX77KQmM3ntTBU8RcgXkXM6S4KmBWtMp7jYsuv2wOZ82oZzCHLx
LEGa5FQxAoGBAPzFO6zz5nv+o9vHbgTJeYqiWW7ZfGxaUHx9fn1uMkRObCYB5T3p
zg9/i9qEaM00M9mVuFVh7KGupmOG+SJO0ohgLM9JLVWrKPtYFJoLcLiViuuI1tqN
2VaxbiaPgyyH5iKtN+qMUyNZNnPKQBraR0wtksdfq3WjlQHlY7CwdTL1AoGBAPsE
0VReGCqMcLWtmX58uQV7wUp4Uo5C6NvpdOu3qoVT5MdNPoQ94tQzIBYxJos92McN
PpaUwW+hN3eI7+A4nSlQfZpm6zfRIf2W2QXnxrh1kEoI74PLu5dQb5kEhzPnia8Z
dFZBGHhkFSAoeHHj4odE5zJA4X97SAR4/ezUm3HFAoGAYLRQlWWmi0+rftz4kWpH
Tqu4DXm1aXrQCiC08OXk408hfxUAkERP6owAhHRd6AwNvjPSlw/kySKZU8CCtgZz
82rgGdnEN19JSYe0Mg8oa+YChQQQNwNH4Cv3mVcUd0fy2tdsahaPhBmQhJIfHmbe
Gh3hZteKMHKKAnSxRBrua7UCgYAmuCaF6gG2RnV4QK/lh92OT/mPwCGT2hRLH6au
WWVPLZKjf3G+OkGL09sGMUytu7t9cLiPx6BS1L5WjOPbZUAwyD6nm2p68KJamHMf
axIHD/NSyDAR11I3U+vy4XTtd7CQNs/wIhhhMabkj/mML8eYb/Ntl/5wiJaiQfuB
FYZotQKBgGxIq3vJTFqk9I761WwMZ+85b0vrEinbU4NDkEhyh1DJuzYhYcZ/nQtr
JPrUkZ5/p6wZhdS2Se0algcOe7osNKTAzzJXKKnjQPSmYF77U6b+gbs/DPbJEUB9
Xn6gLvtGDdDNCbNDAKK2O1mfwZrqBO6vQ+d6nGiFPO74OIoLUgIT
-----END RSA PRIVATE KEY-----"
    };

    public static readonly RsaKeyInfo TestKey3 = new()
    {
        Kid = "TestKey3Kid",
        Public = @"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAo560Ar8VWa6J0r2UNl2C
lmQ/8CQUwUIUeTNHtwR/fYvUB09ymmCzHs1VqPriKnpRgjmgyN7wTZRPMAktWz66
2d0ZBVWIl45XczRLSVOUBj+qnka2xp8sLjPq8AJciGZGSU+EqH6EaMJPk63Xn51R
AypIoRmaUHGtoE1Ddun9gucXwxGYXpXRgEwWr3W2TYPMdHsgl2TeUvNtyDnSgwhF
xaxC9HjqhLCd7Fe1OUsmglWkbsI6Eeymu2+L+K8f9FeGpxd3iNOcvxXdehgNAAhE
NzfCk7Y+5OxoWX4vIOVTid+W/JzvN68uUfHcdj+ABWtOdYLQr6OdBVdk/qwoYmPw
DQIDAQAB
-----END PUBLIC KEY-----",

        Private = @"-----BEGIN RSA PRIVATE KEY-----
MIIEowIBAAKCAQEAo560Ar8VWa6J0r2UNl2ClmQ/8CQUwUIUeTNHtwR/fYvUB09y
mmCzHs1VqPriKnpRgjmgyN7wTZRPMAktWz662d0ZBVWIl45XczRLSVOUBj+qnka2
xp8sLjPq8AJciGZGSU+EqH6EaMJPk63Xn51RAypIoRmaUHGtoE1Ddun9gucXwxGY
XpXRgEwWr3W2TYPMdHsgl2TeUvNtyDnSgwhFxaxC9HjqhLCd7Fe1OUsmglWkbsI6
Eeymu2+L+K8f9FeGpxd3iNOcvxXdehgNAAhENzfCk7Y+5OxoWX4vIOVTid+W/Jzv
N68uUfHcdj+ABWtOdYLQr6OdBVdk/qwoYmPwDQIDAQABAoIBAQCUrsjhngamwkaX
7JJcfMkIJP09GWyK2aOFJ6ZJl5rK2V/JbNa1lp3upaTrs2AGPbUH45pKLtyrgtQa
0JwYyH4/L/1mrX08S6sIE0M+1OY0b3J59cRt5YLeUWtywpKXmGjSgaIJAz8Le1eY
2WaRswFFIQ1y57Mk2shuXQerbaJjamSh+Z5Ve8qISTOUVurAXB7vOZXnWS2Xwdye
OTQ3WUONxkd4zmfzBesfH0BODMW/KPYBi9747zbKMJkxY0CBdezleSGrjkQR2cPD
94IRbWiOhLz01aRb5Gy/2cmPo0sFO5HeXjdwuN93+wVCoQ5jQzPCD3hmCoAmqtCa
dfjasZZ5AoGBAPr2MTo7l72nPkSdjs2kW1qd6GX8RQ8KyGVe5pk2iHtgJAJQx0lC
glq4o9++kDIfpTEjfM8GYTMtXXZRvGXJMUe0iXavIAztVa1/exOFcn3xscbHeGPa
X3YIsiP5g2BRiUk9T64T8Q8WgQnreg3LYnlRMmoR03E8P+3Lh7/7lWV7AoGBAKbn
nxIDEgXnHsZhMTOUib4f0cSDuf0RGLXlDq0XuRQoDdkmWL1kIbxXWFPNOYqP98bS
l503El56IuLgfNEIvcvm3MH5z/f7pVKcG1qScdRaqsGYqgnE6wyWbHzfOx0PgZyi
xGVUeHDILOhSBQBP2PM+c/c0+X0RQLN8mmhqedYXAoGAWkHEvp7EGg9WE7NespLT
0ClOda9e3uQP4nVZFe1I9KZFDRgkNDLZUuto9IVCsuQysxOCRaqTtqYwIOeQjELp
mFJTxf2B12RcFgPtLb4mySjtuQHwSGk3P+ost0d7el6Ys2B7ka3WWojLa9fmk6N8
DQmoygETPHxguZMjhWZj+pUCgYBYYa7KSvXwSkLxqeXz+mQ2WYMrHNYQPE1phLIQ
1s8a7Vrn8nZrU+rJYTdUqQgfZ709e6btiYwgm2x914JA7vASWV3tGewW1Rrmc+PI
QmQknx5tBfqE0dHmCIHKXitfaCaqW0bEocZlcprVKxma4K1xoVCnR1YV9FzerszV
Hg0GYQKBgFJiELfz0tBgzyKFBPauRAGlvEGdKWnSh+d7c3lrA18ozBZ1RpcTdGFP
CrGFYfYx7LbGuhB7web1zIx2VevHRROw6IF6/rQ6G5vPh1FXrffmn67uMRrJpVlh
aLshInBDPtkvD1ozB6HKFj5MZ4265WVbsx1/guA61JpVojWJfn94
-----END RSA PRIVATE KEY-----"
    };
}

