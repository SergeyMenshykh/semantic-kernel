// Copyright (c) Microsoft. All rights reserved.

namespace GroundedInferenceAPI.Options;

/// <summary>
/// Helper class to load application configuration.
/// </summary>
public sealed class ApplicationConfig
{
    private readonly AzureOpenAIConfig _azureOpenAIConfig;
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
            .Bind(this.OpenAIConfig);
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
    public OpenAIConfig OpenAIConfig { get; set; } = new();

    /// <summary>
    /// Gets the RAG configuration.
    /// </summary>
    public RagConfig RagConfig => this._ragConfig;
}
