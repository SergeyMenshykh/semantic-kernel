// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.SemanticKernel;

/// <summary>
/// Shared helpers for local file-system path canonicalization and allowlist validation.
/// </summary>
internal static class LocalPathHelper
{
    // Use case-insensitive comparison on Windows (case-insensitive FS), case-sensitive on Linux/macOS.
    private static readonly StringComparison s_pathComparison =
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;

    /// <summary>
    /// Expands environment variables and resolves the path to its canonical form.
    /// Rejects UNC paths both before and after environment-variable expansion.
    /// This must be called before validation to prevent validate/use mismatches.
    /// </summary>
    /// <param name="path">The raw file path (may contain environment variables).</param>
    /// <returns>A fully qualified, canonical file path.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is null/whitespace or resolves to a UNC path.</exception>
    internal static string CanonicalizePath(string path)
    {
        Verify.NotNullOrWhiteSpace(path);

        if (IsUncPath(path))
        {
            throw new ArgumentException("Invalid file path, UNC paths are not supported.", nameof(path));
        }

        // Expand environment variables first, then canonicalize — so that
        // validation and I/O operate on the same resolved path.
        var expanded = Environment.ExpandEnvironmentVariables(path);

        // Re-check after expansion: an env var could have expanded to a UNC
        // or extended-path prefix (e.g., %NETSHARE% → \\server\share).
        if (IsUncPath(expanded))
        {
            throw new ArgumentException("Invalid file path, UNC paths are not supported.", nameof(path));
        }

        return Path.GetFullPath(expanded);
    }

    /// <summary>
    /// Checks whether a canonicalized file path falls within one of the allowed directories.
    /// Subdirectories of allowed directories are also permitted.
    /// </summary>
    /// <param name="canonicalPath">A fully qualified, canonical file path (call <see cref="CanonicalizePath"/> first).</param>
    /// <param name="allowedDirectories">The set of allowed directory paths.</param>
    /// <returns><c>true</c> if the path's directory is within the allowlist; <c>false</c> otherwise.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="canonicalPath"/> has no directory component.</exception>
    internal static bool IsPathWithinAllowedDirectories(string canonicalPath, IReadOnlyCollection<string> allowedDirectories)
    {
        Verify.NotNullOrWhiteSpace(canonicalPath);

        string? directoryPath = Path.GetDirectoryName(canonicalPath);

        if (string.IsNullOrEmpty(directoryPath))
        {
            throw new ArgumentException("Invalid file path, a fully qualified file location must be specified.", nameof(canonicalPath));
        }

        if (allowedDirectories is null || allowedDirectories.Count == 0)
        {
            return false;
        }

        foreach (var allowedDirectory in allowedDirectories)
        {
            var canonicalAllowed = Path.GetFullPath(allowedDirectory);
            var separator = Path.DirectorySeparatorChar.ToString();
            if (!canonicalAllowed.EndsWith(separator, s_pathComparison))
            {
                canonicalAllowed += separator;
            }

            if (directoryPath.StartsWith(canonicalAllowed, s_pathComparison)
                || (directoryPath + separator).Equals(canonicalAllowed, s_pathComparison))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether the given path is a UNC path (starts with <c>\\</c> or <c>//</c>).
    /// </summary>
    private static bool IsUncPath(string path)
    {
        return path.StartsWith("\\\\", StringComparison.Ordinal)
            || path.StartsWith("//", StringComparison.Ordinal);
    }
}
