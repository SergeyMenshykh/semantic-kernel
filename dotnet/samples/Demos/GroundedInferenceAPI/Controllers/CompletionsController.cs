// Copyright (c) Microsoft. All rights reserved.

using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;

namespace GroundedInferenceAPI.Controllers;

/// <summary>
/// Controller for completions.
/// </summary>
[ApiController]
[Route("[controller]")]
public class CompletionsController : ControllerBase
{
    private readonly ILogger<CompletionsController> _logger;
    private readonly Kernel _kernel;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompletionsController"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="kernel">The kernel.</param>
    public CompletionsController(ILogger<CompletionsController> logger, Kernel kernel)
    {
        this._logger = logger;
        this._kernel = kernel;
    }

    /// <summary>
    /// Get completion for a given prompt
    /// </summary>
    /// <param name="prompt">The prompt to complete</param>
    /// <param name="cancellationToken">The cancellation token</param>
    [HttpPost("Complete/{prompt}")]
    public async Task<string> CompleteAsync(string prompt, CancellationToken cancellationToken)
    {
        FunctionResult result = await this._kernel.InvokePromptAsync(prompt, cancellationToken: cancellationToken).ConfigureAwait(false);

        return result.ToString();
    }

    /// <summary>
    /// Get streaming completion for a given prompt
    /// </summary>
    /// <param name="prompt">The prompt to complete</param>
    /// <param name="cancellationToken">The cancellation token</param>
    [HttpPost("CompleteStreaming/{prompt}")]
    public async IAsyncEnumerable<string> CompleteStreamingAsync(string prompt, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        IAsyncEnumerable<StreamingKernelContent> content = this._kernel.InvokePromptStreamingAsync(prompt, cancellationToken: cancellationToken);

        await foreach (StreamingKernelContent item in content.ConfigureAwait(false))
        {
            yield return item.ToString();
        }
    }
}
