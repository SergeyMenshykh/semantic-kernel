﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using Azure.Core;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Services;
using Moq;

namespace SemanticKernel.Connectors.AzureOpenAI.UnitTests.Services;

/// <summary>
/// Unit tests for <see cref="AzureOpenAIAudioToTextService"/> class.
/// </summary>
public sealed class AzureOpenAIAudioToTextServiceTests : IDisposable
{
    private readonly HttpMessageHandlerStub _messageHandlerStub;
    private readonly HttpClient _httpClient;
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;

    public AzureOpenAIAudioToTextServiceTests()
    {
        this._messageHandlerStub = new HttpMessageHandlerStub();
        this._httpClient = new HttpClient(this._messageHandlerStub, false);
        this._mockLoggerFactory = new Mock<ILoggerFactory>();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ConstructorWithApiKeyWorksCorrectly(bool includeLoggerFactory)
    {
        // Arrange & Act
        var service = includeLoggerFactory ?
            new AzureOpenAIAudioToTextService("deployment", "https://endpoint", "api-key", "model-id", loggerFactory: this._mockLoggerFactory.Object) :
            new AzureOpenAIAudioToTextService("deployment", "https://endpoint", "api-key", "model-id");

        // Assert
        Assert.Equal("model-id", service.Attributes[AIServiceExtensions.ModelIdKey]);
        Assert.Equal("deployment", service.Attributes[ClientCore.DeploymentNameKey]);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ConstructorWithTokenCredentialWorksCorrectly(bool includeLoggerFactory)
    {
        // Arrange & Act
        var credentials = DelegatedTokenCredential.Create((_, _) => new AccessToken());
        var service = includeLoggerFactory ?
            new AzureOpenAIAudioToTextService("deployment", "https://endpoint", credentials, "model-id", loggerFactory: this._mockLoggerFactory.Object) :
            new AzureOpenAIAudioToTextService("deployment", "https://endpoint", credentials, "model-id");

        // Assert
        Assert.Equal("model-id", service.Attributes[AIServiceExtensions.ModelIdKey]);
        Assert.Equal("deployment", service.Attributes[ClientCore.DeploymentNameKey]);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ConstructorWithOpenAIClientWorksCorrectly(bool includeLoggerFactory)
    {
        // Arrange & Act
        var client = new AzureOpenAIClient(new Uri("http://host"), "key");
        var service = includeLoggerFactory ?
            new AzureOpenAIAudioToTextService("deployment", client, "model-id", loggerFactory: this._mockLoggerFactory.Object) :
            new AzureOpenAIAudioToTextService("deployment", client, "model-id");

        // Assert
        Assert.Equal("model-id", service.Attributes[AIServiceExtensions.ModelIdKey]);
        Assert.Equal("deployment", service.Attributes[ClientCore.DeploymentNameKey]);
    }

    [Fact]
    public void ItThrowsIfDeploymentNameIsNotProvided()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new AzureOpenAIAudioToTextService(" ", "http://host", "apikey"));
        Assert.Throws<ArgumentException>(() => new AzureOpenAIAudioToTextService(" ", azureOpenAIClient: new(new Uri("http://host"), "apikey")));
        Assert.Throws<ArgumentException>(() => new AzureOpenAIAudioToTextService("", "http://host", "apikey"));
        Assert.Throws<ArgumentException>(() => new AzureOpenAIAudioToTextService("", azureOpenAIClient: new(new Uri("http://host"), "apikey")));
        Assert.Throws<ArgumentNullException>(() => new AzureOpenAIAudioToTextService(null!, "http://host", "apikey"));
        Assert.Throws<ArgumentNullException>(() => new AzureOpenAIAudioToTextService(null!, azureOpenAIClient: new(new Uri("http://host"), "apikey")));
    }

    [Theory]
    [MemberData(nameof(ExecutionSettings))]
    public async Task GetTextContentWithInvalidSettingsThrowsExceptionAsync(AzureOpenAIAudioToTextExecutionSettings? settings, Type expectedExceptionType)
    {
        // Arrange
        var service = new AzureOpenAIAudioToTextService("deployment", "https://endpoint", "api-key", "model-id", this._httpClient);
        this._messageHandlerStub.ResponseToReturn = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent("Test audio-to-text response")
        };

        // Act
        var exception = await Record.ExceptionAsync(() => service.GetTextContentsAsync(new AudioContent(new BinaryData("data"), mimeType: null), settings));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType(expectedExceptionType, exception);
    }

    [Theory]
    [InlineData(AzureOpenAIAudioToTextExecutionSettings.AudioTranscriptionFormat.Verbose, "verbose_json")]
    [InlineData(AzureOpenAIAudioToTextExecutionSettings.AudioTranscriptionFormat.Simple, "json")]
    [InlineData(AzureOpenAIAudioToTextExecutionSettings.AudioTranscriptionFormat.Vtt, "vtt")]
    [InlineData(AzureOpenAIAudioToTextExecutionSettings.AudioTranscriptionFormat.Srt, "srt")]
    public async Task ItRespectResultFormatExecutionSettingAsync(AzureOpenAIAudioToTextExecutionSettings.AudioTranscriptionFormat responseFormat, string expectedFormat)
    {
        // Arrange
        var service = new AzureOpenAIAudioToTextService("deployment", "https://endpoint", "api-key", httpClient: this._httpClient);
        this._messageHandlerStub.ResponseToReturn = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent("Test audio-to-text response")
        };

        // Act
        var settings = new AzureOpenAIAudioToTextExecutionSettings("file.mp3") { ResponseFormat = responseFormat };
        var result = await service.GetTextContentsAsync(new AudioContent(new BinaryData("data"), mimeType: null), settings);

        // Assert
        Assert.NotNull(this._messageHandlerStub.RequestContent);
        Assert.NotNull(result);

        var multiPartData = Encoding.UTF8.GetString(this._messageHandlerStub.RequestContent!);
        var multiPartBreak = multiPartData.Substring(0, multiPartData.IndexOf("\r\n", StringComparison.OrdinalIgnoreCase));

        Assert.Contains($"{expectedFormat}\r\n{multiPartBreak}", multiPartData);
    }

    [Fact]
    public async Task GetTextContentByDefaultWorksCorrectlyAsync()
    {
        // Arrange
        var service = new AzureOpenAIAudioToTextService("deployment-name", "https://endpoint", "api-key", "model-id", this._httpClient);
        this._messageHandlerStub.ResponseToReturn = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent("Test audio-to-text response")
        };

        // Act
        var result = await service.GetTextContentsAsync(new AudioContent(new BinaryData("data"), mimeType: null), new AzureOpenAIAudioToTextExecutionSettings("file.mp3"));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test audio-to-text response", result[0].Text);
    }

    public void Dispose()
    {
        this._httpClient.Dispose();
        this._messageHandlerStub.Dispose();
    }

    public static TheoryData<AzureOpenAIAudioToTextExecutionSettings?, Type> ExecutionSettings => new()
    {
        { new AzureOpenAIAudioToTextExecutionSettings(""), typeof(ArgumentException) },
        { new AzureOpenAIAudioToTextExecutionSettings("file"), typeof(ArgumentException) }
    };
}
