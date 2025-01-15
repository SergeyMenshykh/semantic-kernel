// Copyright (c) Microsoft. All rights reserved.

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.SemanticKernel;

/// <summary>
/// Represent function policies.
/// </summary>
[Experimental("SKEXP0001")]
public sealed class FunctionPolicies
{
    /// <summary>
    /// Gets or sets policy for handling function names.
    /// </summary>
    public FunctionNamePolicy FunctionNamePolicy { get; set; } = new FunctionNamePolicy();
}
