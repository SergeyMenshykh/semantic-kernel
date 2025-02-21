// Copyright (c) Microsoft. All rights reserved.

namespace GroundedInferenceAPI.Controllers;

/// <summary>
/// The prompt request model.
/// </summary>
public class PromptRequest
{
    /// <summary>
    /// Gets or sets the prompt.
    /// </summary>
    public required string Prompt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether streaming is requested.
    /// </summary>
    public bool IsStreaming { get; set; }
}
