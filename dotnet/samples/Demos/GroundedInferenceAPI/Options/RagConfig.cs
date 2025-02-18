// Copyright (c) Microsoft. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace GroundedInferenceAPI.Config;

/// <summary>
/// Contains settings to control the RAG experience.
/// </summary>
public sealed class RagConfig
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string ConfigSectionName = "Rag";

    /// <summary>
    /// The name of the AI chat service.
    /// </summary>
    [Required]
    public string AIChatService { get; set; } = string.Empty;
}
