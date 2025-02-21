// Copyright (c) Microsoft. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace GroundedInferenceAPI.Options;

/// <summary>
/// Azure OpenAI Embeddings service settings.
/// </summary>
public sealed class AzureOpenAIEmbeddingsConfig
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string ConfigSectionName = "AzureOpenAIEmbeddings";

    /// <summary>
    /// The name of the deployment.
    /// </summary>
    [Required]
    public string DeploymentName { get; set; } = string.Empty;

    /// <summary>
    /// The endpoint of the Azure OpenAI Embeddings service.
    /// </summary>
    [Required]
    public string Endpoint { get; set; } = string.Empty;
}
