// Copyright (c) Microsoft. All rights reserved.

using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;

namespace GroundedInferenceAPI.Controllers;

/// <summary>
/// Controller for completions.
/// </summary>
[ApiController]
[Route("chat/completions")]
[Consumes("application/json")]
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
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    [HttpPost]
    public async Task<IActionResult> CompleteAsync([FromBody] PromptRequest request, CancellationToken cancellationToken)
    {
        if (request.IsStreaming)
        {
            return this.Ok(this.CompleteStreamingAsync(request, cancellationToken));
        }

        FunctionResult result = await this._kernel.InvokePromptAsync(request.Prompt, cancellationToken: cancellationToken).ConfigureAwait(false);

        return this.Ok(result.ToString());
    }

    private async IAsyncEnumerable<string> CompleteStreamingAsync([FromBody] PromptRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        IAsyncEnumerable<StreamingKernelContent> content = this._kernel.InvokePromptStreamingAsync(request.Prompt, cancellationToken: cancellationToken);

        await foreach (StreamingKernelContent item in content.ConfigureAwait(false))
        {
            yield return item.ToString();
        }
    }
}
