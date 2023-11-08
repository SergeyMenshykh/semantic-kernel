// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.SemanticKernel.TemplateEngine.Basic.Blocks;

/// <summary>
/// Interface of dynamic blocks that need async IO to be rendered.
/// </summary>
public interface ICodeRendering
{
    /// <summary>
    /// Render the block using the given context, potentially using external I/O.
    /// </summary>
    /// <param name="kernel">Kernel to use for rendering.</param></param>
    /// <param name="arguments">Arguments providing values that can be used in the template.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    /// <returns>Rendered content</returns>
    public Task<string> RenderCodeAsync(Kernel kernel, IReadOnlyDictionary<string, object?>? arguments, CancellationToken cancellationToken = default);
}
