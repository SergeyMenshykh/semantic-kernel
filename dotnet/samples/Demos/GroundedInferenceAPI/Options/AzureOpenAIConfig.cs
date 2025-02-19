// Copyright (c) Microsoft. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace GroundedInferenceAPI.Options;

/// <summary>
/// Azure OpenAI service settings.
/// </summary>
public sealed class AzureOpenAIConfig
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string ConfigSectionName = "AzureOpenAI";

    /// <summary>
    /// The name of the chat deployment.
    /// </summary>
    [Required]
    public string ChatDeploymentName { get; set; } = string.Empty;

    /// <summary>
    /// The endpoint of the Azure OpenAI service.
    /// </summary>
    [Required]
    public string Endpoint { get; set; } = string.Empty;
}
