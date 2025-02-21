// Copyright (c) Microsoft. All rights reserved.

using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;

namespace GroundedInferenceAPI.Controllers;

/// <summary>
/// Controller for managing embeddings
/// </summary>
[ApiController]
[Route("embeddings")]
public class EmbeddingsController : ControllerBase
{
    private readonly ILogger<EmbeddingsController> _logger;
    private readonly Kernel _kernel;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddingsController"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="kernel">The kernel.</param>
    public EmbeddingsController(ILogger<EmbeddingsController> logger, Kernel kernel)
    {
        this._logger = logger;
        this._kernel = kernel;
    }

    /// <summary>
    /// Upload a document.
    /// </summary>
    /// <param name="document">The document to upload.</param>
    [HttpPost]
    public async Task<IActionResult> CreateEmbeddingAsync(IFormFile document)
    {
        if (document == null || document.Length == 0)
        {
            return this.BadRequest("No file uploaded.");
        }

        return this.Ok(new { document.FileName });
    }
}
