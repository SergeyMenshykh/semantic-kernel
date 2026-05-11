// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.SemanticKernel;
using Xunit;

namespace SemanticKernel.Plugins.UnitTests.IO;

public class LocalPathHelperTests
{
    [Fact]
    public void CanonicalizePathReturnsFullPath()
    {
        // Arrange
        var tempDir = Path.GetTempPath();
        var filePath = Path.Combine(tempDir, "test-file.txt");

        // Act
        var result = LocalPathHelper.CanonicalizePath(filePath);

        // Assert
        Assert.Equal(Path.GetFullPath(filePath), result);
    }

    [Fact]
    public void CanonicalizePathResolvesRelativePaths()
    {
        // Act
        var result = LocalPathHelper.CanonicalizePath("myfile.txt");

        // Assert — should resolve to absolute path
        Assert.True(Path.IsPathRooted(result));
    }

    [Fact]
    public void CanonicalizePathCollapsesTraversalSegments()
    {
        // Arrange
        var tempDir = Path.GetTempPath();
        var traversalPath = Path.Combine(tempDir, "subdir", "..", "test-file.txt");

        // Act
        var result = LocalPathHelper.CanonicalizePath(traversalPath);

        // Assert — ".." should be collapsed
        Assert.Equal(Path.GetFullPath(Path.Combine(tempDir, "test-file.txt")), result);
    }

    [Fact]
    public void CanonicalizePathExpandsEnvironmentVariables()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return; // %TEMP% is a Windows environment variable
        }

        // Arrange
        var envVarPath = Path.Combine("%TEMP%", "test-file.txt");

        // Act
        var result = LocalPathHelper.CanonicalizePath(envVarPath);

        // Assert — should expand the environment variable
        Assert.DoesNotContain("%TEMP%", result);
        Assert.True(Path.IsPathRooted(result));
    }

    [Theory]
    [InlineData("\\\\server\\share\\file.txt")]
    [InlineData("//server/share/file.txt")]
    public void CanonicalizePathRejectsUncPaths(string uncPath)
    {
        Assert.Throws<ArgumentException>(() => LocalPathHelper.CanonicalizePath(uncPath));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CanonicalizePathRejectsWhitespace(string path)
    {
        Assert.Throws<ArgumentException>(() => LocalPathHelper.CanonicalizePath(path));
    }

    [Fact]
    public void CanonicalizePathRejectsNull()
    {
        Assert.ThrowsAny<ArgumentException>(() => LocalPathHelper.CanonicalizePath(null!));
    }

    [Fact]
    public void CanonicalizePathRejectsUncAfterEnvVarExpansion()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return; // Environment variable expansion behaves differently on non-Windows
        }

        // Arrange
        var envVarName = $"SK_TEST_UNC_{Guid.NewGuid():N}";
        Environment.SetEnvironmentVariable(envVarName, @"\\server\share");
        try
        {
            var envVarPath = $"%{envVarName}%\\file.txt";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => LocalPathHelper.CanonicalizePath(envVarPath));
        }
        finally
        {
            Environment.SetEnvironmentVariable(envVarName, null);
        }
    }

    [Fact]
    public void IsPathWithinAllowedDirectoriesReturnsTrueForAllowedPath()
    {
        // Arrange
        var tempDir = Path.GetTempPath();
        var filePath = Path.GetFullPath(Path.Combine(tempDir, "test-file.txt"));
        var allowed = new HashSet<string> { tempDir };

        // Act & Assert
        Assert.True(LocalPathHelper.IsPathWithinAllowedDirectories(filePath, allowed));
    }

    [Fact]
    public void IsPathWithinAllowedDirectoriesReturnsTrueForSubdirectory()
    {
        // Arrange
        var tempDir = Path.GetTempPath();
        var filePath = Path.GetFullPath(Path.Combine(tempDir, "subdir", "deep", "test-file.txt"));
        var allowed = new HashSet<string> { tempDir };

        // Act & Assert
        Assert.True(LocalPathHelper.IsPathWithinAllowedDirectories(filePath, allowed));
    }

    [Fact]
    public void IsPathWithinAllowedDirectoriesReturnsFalseForDisallowedPath()
    {
        // Arrange
        var allowedDir = Path.Combine(Path.GetTempPath(), "allowed-dir");
        var disallowedPath = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "disallowed", "file.txt"));
        var allowed = new HashSet<string> { allowedDir };

        // Act & Assert
        Assert.False(LocalPathHelper.IsPathWithinAllowedDirectories(disallowedPath, allowed));
    }

    [Fact]
    public void IsPathWithinAllowedDirectoriesReturnsFalseForEmptyAllowlist()
    {
        // Arrange
        var filePath = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "file.txt"));
        var empty = new HashSet<string>();

        // Act & Assert
        Assert.False(LocalPathHelper.IsPathWithinAllowedDirectories(filePath, empty));
    }

    [Fact]
    public void IsPathWithinAllowedDirectoriesReturnsFalseForNullAllowlist()
    {
        // Arrange
        var filePath = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "file.txt"));

        // Act & Assert
        Assert.False(LocalPathHelper.IsPathWithinAllowedDirectories(filePath, null!));
    }

    [Fact]
    public void IsPathWithinAllowedDirectoriesPreventsTraversal()
    {
        // Arrange
        var allowedDir = Path.Combine(Path.GetTempPath(), "allowed");
        var traversalPath = Path.GetFullPath(Path.Combine(allowedDir, "..", "outside", "file.txt"));
        var allowed = new HashSet<string> { allowedDir };

        // Act & Assert — canonicalized traversal path should be outside allowed dir
        Assert.False(LocalPathHelper.IsPathWithinAllowedDirectories(traversalPath, allowed));
    }
}
