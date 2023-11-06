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
namespace SecuredApi.Logic.Subscriptions;

public class Sha256HashCalculatorTests
{
    private Sha256HashCalculator _sut
        = new(Options.Create<SubscriptionsSecurityCfg>(new()
            {
                Salt = "5b951d0869cc4d2da993b6d188197c71"
            }));

    [Fact]
    public void CalculateHash_ValidInput_HashCalculated()
    {
        const string key = "5F39D492-A141-498A-AE04-76C6B77F246A";

        var hash = _sut.CalculateHash(key);

        hash.Should().Be("70C0B884A57C5B7171DE377C777B159B55CCA11E9F5DD8CD9D8DFD2DAF735074");
        //NUYzOUQ0OTItQTE0MS00OThBLUFFMDQtNzZDNkI3N0YyNDZBNWI5NTFkMDg2OWNjNGQyZGE5OTNiNmQxODgxOTdjNzE=
    }
}

