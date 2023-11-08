// Copyright (c) Microsoft. All rights reserved.

using System;
using Microsoft.SemanticKernel.Diagnostics;

namespace Microsoft.SemanticKernel.Events;

/// <summary>
/// Base arguments for events.
/// </summary>
public abstract class KernelEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KernelEventArgs"/> class.
    /// </summary>
    /// <param name="functionView">Function view details</param>
    /// <param name="functionArguments">Function arguments</param>
    internal KernelEventArgs(FunctionView functionView, KernelFunctionParameters functionArguments)
    {
        Verify.NotNull(functionView);
        Verify.NotNull(functionArguments);

        this.FunctionView = functionView;
        this.Arguments = functionArguments;
    }

    /// <summary>
    /// Function view details.
    /// </summary>
    public FunctionView FunctionView { get; }

    /// <summary>
    /// Context related to the event.
    /// </summary>
    public KernelFunctionParameters Arguments { get; }
}
