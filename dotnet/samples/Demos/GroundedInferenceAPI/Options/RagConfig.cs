// Copyright (c) Microsoft. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace GroundedInferenceAPI.Options;

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

    /// <summary>
    /// The name of the AI embedding service.
    /// </summary>
    [Required]
    public string AIEmbeddingService { get; set; } = string.Empty;

    /// <summary>
    /// The name of the vector store.
    /// </summary>
    [Required]
    public string VectorStore { get; set; } = string.Empty;

    /// <summary>
    /// The name of the collection.
    /// </summary>
    [Required]
    public string CollectionName { get; set; } = string.Empty;
}
