// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;

namespace Microsoft.SemanticKernel;

/// <summary>
/// Shared helpers for remote (cloud) path normalization and allowlist validation.
/// </summary>
internal static class RemotePathHelper
{
    /// <summary>
    /// Normalizes a remote path by replacing backslashes with forward slashes
    /// and collapsing "." and ".." segments to prevent traversal bypass.
    /// </summary>
    /// <param name="path">The raw remote path.</param>
    /// <returns>A normalized path starting with "/" and using forward slashes only.</returns>
    internal static string NormalizePath(string path)
    {
        var normalizedPath = path.Replace('\\', '/');

        // Collapse ".." and "." segments to prevent traversal bypass.
        var segments = normalizedPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        var stack = new List<string>();
        foreach (var segment in segments)
        {
            if (segment == ".." && stack.Count > 0)
            {
                stack.RemoveAt(stack.Count - 1);
            }
            else if (segment != "." && segment != "..")
            {
                stack.Add(segment);
            }
        }

        return "/" + string.Join("/", stack);
    }

    /// <summary>
    /// Checks whether the provided remote path falls within one of the allowed remote directory prefixes.
    /// Paths are normalized with forward slashes, dot-segments are collapsed,
    /// and compared case-insensitively (cloud storage paths are typically case-insensitive).
    /// Subdirectories of allowed paths are permitted.
    /// </summary>
    /// <param name="path">The remote path to check.</param>
    /// <param name="allowedPrefixes">The set of allowed remote directory prefixes.</param>
    /// <returns><c>true</c> if the path's parent directory falls within the allowlist; <c>false</c> otherwise.</returns>
    internal static bool IsPathWithinAllowedPrefixes(string path, IReadOnlyCollection<string> allowedPrefixes)
    {
        Verify.NotNullOrWhiteSpace(path);

        if (allowedPrefixes is null || allowedPrefixes.Count == 0)
        {
            return false;
        }

        // Normalize to forward slashes and collapse dot-segments to prevent traversal bypass.
        var normalizedPath = NormalizePath(path);

        foreach (var allowedPrefix in allowedPrefixes)
        {
            var normalizedAllowed = NormalizePath(allowedPrefix);
            if (!normalizedAllowed.EndsWith("/", StringComparison.Ordinal))
            {
                normalizedAllowed += "/";
            }

            var normalizedDir = normalizedPath;
            int lastSlash = normalizedDir.LastIndexOf('/');
            if (lastSlash >= 0)
            {
                normalizedDir = normalizedDir.Substring(0, lastSlash);
            }

            if ((normalizedDir + "/").StartsWith(normalizedAllowed, StringComparison.OrdinalIgnoreCase)
                || (normalizedDir + "/").Equals(normalizedAllowed, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
