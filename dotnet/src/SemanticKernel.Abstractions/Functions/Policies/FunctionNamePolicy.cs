// Copyright (c) Microsoft. All rights reserved.

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.SemanticKernel;

/// <summary>
/// Represents a function name policy.
/// </summary>
[Experimental("SKEXP0001")]
public sealed class FunctionNamePolicy
{
    /// <summary>
    /// Gets or sets a value indicating whether to use only the function name and ignore the plugin name for function advertising and lookup.
    /// </summary>
    public bool UseFunctionNameOnly { get; init; } = false;

    /// <summary>
    /// Gets or sets the function name separator.
    /// </summary>
    public string FunctionNameSeparator { get; init; } = "-";
}
