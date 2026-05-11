// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.SemanticKernel;
using Xunit;

namespace SemanticKernel.Plugins.UnitTests.IO;

public class RemotePathHelperTests
{
    [Fact]
    public void NormalizePathReplacesBackslashesWithForwardSlashes()
    {
        Assert.Equal("/Documents/Reports/file.txt", RemotePathHelper.NormalizePath(@"\Documents\Reports\file.txt"));
    }

    [Fact]
    public void NormalizePathCollapsesDotDotSegments()
    {
        Assert.Equal("/Documents/file.txt", RemotePathHelper.NormalizePath("/Documents/Reports/../file.txt"));
    }

    [Fact]
    public void NormalizePathCollapsesDotSegments()
    {
        Assert.Equal("/Documents/file.txt", RemotePathHelper.NormalizePath("/Documents/./file.txt"));
    }

    [Fact]
    public void NormalizePathHandlesMultipleTraversals()
    {
        Assert.Equal("/file.txt", RemotePathHelper.NormalizePath("/Documents/Reports/../../file.txt"));
    }

    [Fact]
    public void NormalizePathHandlesTraversalBeyondRoot()
    {
        // Attempting to traverse above root should stay at root
        Assert.Equal("/file.txt", RemotePathHelper.NormalizePath("/../../../file.txt"));
    }

    [Fact]
    public void NormalizePathPrefixesWithSlash()
    {
        Assert.Equal("/Documents/file.txt", RemotePathHelper.NormalizePath("Documents/file.txt"));
    }

    [Fact]
    public void NormalizePathHandlesMixedSeparators()
    {
        Assert.Equal("/Documents/Reports/file.txt", RemotePathHelper.NormalizePath(@"Documents\Reports/file.txt"));
    }

    [Fact]
    public void IsPathWithinAllowedPrefixesReturnsTrueForAllowedPath()
    {
        // Arrange
        var allowed = new HashSet<string> { "/Documents" };

        // Act & Assert
        Assert.True(RemotePathHelper.IsPathWithinAllowedPrefixes("/Documents/file.txt", allowed));
    }

    [Fact]
    public void IsPathWithinAllowedPrefixesReturnsTrueForSubdirectory()
    {
        // Arrange
        var allowed = new HashSet<string> { "/Documents" };

        // Act & Assert
        Assert.True(RemotePathHelper.IsPathWithinAllowedPrefixes("/Documents/Reports/Q1/file.txt", allowed));
    }

    [Fact]
    public void IsPathWithinAllowedPrefixesReturnsFalseForDisallowedPath()
    {
        // Arrange
        var allowed = new HashSet<string> { "/Documents/Public" };

        // Act & Assert
        Assert.False(RemotePathHelper.IsPathWithinAllowedPrefixes("/Confidential/secret.txt", allowed));
    }

    [Fact]
    public void IsPathWithinAllowedPrefixesReturnsFalseForEmptyAllowlist()
    {
        Assert.False(RemotePathHelper.IsPathWithinAllowedPrefixes("/Documents/file.txt", new HashSet<string>()));
    }

    [Fact]
    public void IsPathWithinAllowedPrefixesPreventsTraversal()
    {
        // Arrange
        var allowed = new HashSet<string> { "/Documents/Public" };

        // Act & Assert — traversal should be collapsed and denied
        Assert.False(RemotePathHelper.IsPathWithinAllowedPrefixes("/Documents/Public/../../Confidential/secret.txt", allowed));
    }

    [Fact]
    public void IsPathWithinAllowedPrefixesIsCaseInsensitive()
    {
        // Arrange
        var allowed = new HashSet<string> { "/Documents" };

        // Act & Assert
        Assert.True(RemotePathHelper.IsPathWithinAllowedPrefixes("/DOCUMENTS/file.txt", allowed));
    }

    [Fact]
    public void IsPathWithinAllowedPrefixesHandlesBackslashesInInput()
    {
        // Arrange
        var allowed = new HashSet<string> { "/Documents" };

        // Act & Assert — backslashes should be normalized to forward slashes
        Assert.True(RemotePathHelper.IsPathWithinAllowedPrefixes(@"\Documents\file.txt", allowed));
    }

    [Fact]
    public void IsPathWithinAllowedPrefixesDeniesAllByDefault()
    {
        Assert.False(RemotePathHelper.IsPathWithinAllowedPrefixes("/any/path.txt", new HashSet<string>()));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void IsPathWithinAllowedPrefixesRejectsWhitespace(string path)
    {
        var allowed = new HashSet<string> { "/Documents" };
        Assert.Throws<ArgumentException>(() => RemotePathHelper.IsPathWithinAllowedPrefixes(path, allowed));
    }

    [Fact]
    public void IsPathWithinAllowedPrefixesRejectsNull()
    {
        var allowed = new HashSet<string> { "/Documents" };
        Assert.ThrowsAny<ArgumentException>(() => RemotePathHelper.IsPathWithinAllowedPrefixes(null!, allowed));
    }

    [Fact]
    public void IsPathWithinAllowedPrefixesAllowsRootPrefix()
    {
        // Arrange — allowing root should permit everything
        var allowed = new HashSet<string> { "/" };

        // Act & Assert
        Assert.True(RemotePathHelper.IsPathWithinAllowedPrefixes("/any/path/file.txt", allowed));
    }
}
