// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using Microsoft.SemanticKernel.AI;

namespace Microsoft.SemanticKernel.Functions;

/// <summary>Represent kernel function parameters.</summary>
//TODO: Consider redesigning it.
public class KernelFunctionParameters : Dictionary<string, object?>
{
    /// <summary>Gets or sets the AI request settings associated with this context.</summary>
    public AIRequestSettings? RequestSettings { get; set; }
}
