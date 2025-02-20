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
}
