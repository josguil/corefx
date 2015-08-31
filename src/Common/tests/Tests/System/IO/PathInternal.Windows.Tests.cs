﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using Xunit;

public class PathInternal_Windows_Tests
{
    [Theory]
    [InlineData(PathInternal.ExtendedPathPrefix, PathInternal.ExtendedPathPrefix)]
    [InlineData(@"Foo", @"Foo")]
    [InlineData(@"C:\Foo", @"\\?\C:\Foo")]
    [InlineData(@"\\.\Foo", @"\\.\Foo")]
    [InlineData(@"\\Server\Share", PathInternal.UncExtendedPathPrefix + @"Server\Share")]
    [PlatformSpecific(PlatformID.Windows)]
    public void AddExtendedPathPrefixTest(string path, string expected)
    {
        Assert.Equal(expected, PathInternal.AddExtendedPathPrefix(path));
    }

    [Theory,
        InlineData("", true),
        InlineData("C:", true),
        InlineData("**", true),
        InlineData(@"\\.\path", false),
        InlineData(@"\\?\path", false),
        InlineData(@"\\.", false),
        InlineData(@"\\?", false),
        InlineData(@"\\", false),
        InlineData(@"//", false),
        InlineData(@"\", true),
        InlineData(@"/", true),
        InlineData(@"C:Path", true),
        InlineData(@"C:\Path", false),
        InlineData(@"\\?\C:\Path", false),
        InlineData(@"Path", true),
        InlineData(@"X", true)]
    public void IsPathRelative(string path, bool expected)
    {
        Assert.Equal(expected, PathInternal.IsPathRelative(path));
    }
}
