// Copyright (c) Microsoft. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace GroundedInferenceAPI.Options;

/// <summary>
/// OpenAI service settings.
/// </summary>
public sealed class OpenAIConfig
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string ConfigSectionName = "OpenAI";

    /// <summary>
    /// Get or sets the chat model ID.
    /// </summary>
    [Required]
    public string ChatModelId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the API key.
    /// </summary>
    [Required]
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the organization ID.
    /// </summary>
    [Required]
    public string? OrgId { get; set; } = null;
}
