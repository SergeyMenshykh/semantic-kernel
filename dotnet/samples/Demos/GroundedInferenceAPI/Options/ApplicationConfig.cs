// Copyright (c) Microsoft. All rights reserved.

namespace GroundedInferenceAPI.Options;

/// <summary>
/// Helper class to load application configuration.
/// </summary>
public sealed class ApplicationConfig
{
    private readonly OpenAIConfig _openAIConfig = new();
    private readonly AzureOpenAIConfig _azureOpenAIConfig = new();
    private readonly AzureOpenAIEmbeddingsConfig _azureOpenAIEmbeddingsConfig = new();
    private readonly RagConfig _ragConfig = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationConfig"/> class.
    /// </summary>
    /// <param name="configurationManager">The configuration manager.</param></param>
    public ApplicationConfig(ConfigurationManager configurationManager)
    {
        this._azureOpenAIConfig = new();
        configurationManager
            .GetRequiredSection($"AIServices:{AzureOpenAIConfig.ConfigSectionName}")
            .Bind(this._azureOpenAIConfig);
        configurationManager
            .GetRequiredSection($"AIServices:{OpenAIConfig.ConfigSectionName}")
            .Bind(this._openAIConfig);
        configurationManager
            .GetRequiredSection($"AIServices:{AzureOpenAIEmbeddingsConfig.ConfigSectionName}")
            .Bind(this._azureOpenAIEmbeddingsConfig);
        configurationManager
            .GetRequiredSection(RagConfig.ConfigSectionName)
            .Bind(this._ragConfig);
    }

    /// <summary>
    /// Gets the Azure OpenAI configuration.
    /// </summary>
    public AzureOpenAIConfig AzureOpenAIConfig => this._azureOpenAIConfig;

    /// <summary>
    /// Gets the OpenAI configuration.
    /// </summary>
    public OpenAIConfig OpenAIConfig => this._openAIConfig;

    /// <summary>
    /// Gets the Azure OpenAI embeddings configuration.
    /// </summary>
    public AzureOpenAIEmbeddingsConfig AzureOpenAIEmbeddingsConfig => this._azureOpenAIEmbeddingsConfig;

    /// <summary>
    /// Gets the RAG configuration.
    /// </summary>
    public RagConfig RagConfig => this._ragConfig;
}
