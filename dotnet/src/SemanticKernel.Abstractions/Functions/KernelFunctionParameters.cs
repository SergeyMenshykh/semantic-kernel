// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.SemanticKernel.AI;

#pragma warning disable IDE0130

namespace Microsoft.SemanticKernel;

/// <summary>Represent kernel function parameters.</summary>
//TODO: Consider redesigning it.
public class KernelFunctionParameters : Dictionary<string, object?>
{
    /// <summary>Initializes a new instance of the <see cref="KernelFunctionParameters"/> class.</summary>
    public KernelFunctionParameters() : base(new KernelFunctionParameters())
    {
    }

    /// <summary>Initializes a new instance of the <see cref="KernelFunctionParameters"/> class.</summary>
    public KernelFunctionParameters(IDictionary<string, object?> dictionary) : base(dictionary)
    {
    }

    /// <summary>Gets or sets the AI request settings associated with this context.</summary>
    public AIRequestSettings? RequestSettings { get; set; }
}
