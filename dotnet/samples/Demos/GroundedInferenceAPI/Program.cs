// Copyright (c) Microsoft. All rights reserved.

using Azure.Identity;
using GroundedInferenceAPI.Model;
using GroundedInferenceAPI.Options;
using Microsoft.SemanticKernel;

namespace GroundedInferenceAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ApplicationConfig appConfig = GetApplicationConfiguration(builder);

        builder.Services.AddSingleton<ApplicationConfig>((_) => appConfig);

        builder.Services.AddAuthentication();

        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Add AI services to the container.
        IKernelBuilder kernelBuilder = builder.Services.AddKernel();

        AddAndConfigureAiServices(kernelBuilder, appConfig);

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        // To be removed in production.
        app.UseDeveloperExceptionPage();

        app.Run();
    }

    private static ApplicationConfig GetApplicationConfiguration(WebApplicationBuilder builder)
    {
        if (builder.Environment.IsProduction())
        {
            // Read secrets from Azure Key Vault in production environment.
            var keyVaultEndpoint = Environment.GetEnvironmentVariable("AZURE_KEY_VAULT_ENDPOINT");
            if (string.IsNullOrEmpty(keyVaultEndpoint))
            {
                throw new InvalidOperationException("Azure Key Vault endpoint is not set.");
            }

            builder.Configuration.AddAzureKeyVault(new Uri(keyVaultEndpoint), new DefaultAzureCredential());
        }
        else
        {
            // Read user secrets from secret manager in development environment.
            builder.Configuration.AddUserSecrets<ApplicationConfig>();
        }

        var appConfig = new ApplicationConfig(builder.Configuration);
        if (!builder.Environment.IsProduction())
        {
            // Use chat deployment name deployed and provisioned by azd if no value is already available in user secrets.
            if (string.IsNullOrEmpty(appConfig.AzureOpenAIConfig.ChatDeploymentName))
            {
                appConfig.AzureOpenAIConfig.ChatDeploymentName = builder.Configuration["AZURE_OPENAI_CHAT_MODEL_NAME"] ?? string.Empty;
            }

            // Use chat endpoint of Azure AI Service deployed and provisioned by azd if no value is already available in user secrets.
            if (string.IsNullOrEmpty(appConfig.AzureOpenAIConfig.Endpoint))
            {
                appConfig.AzureOpenAIConfig.Endpoint = builder.Configuration["AZURE_AI_SERVICE_ENDPOINT"] ?? string.Empty;
            }
        }

        return appConfig;
    }

    private static void AddAndConfigureAiServices(IKernelBuilder kernelBuilder, ApplicationConfig appConfig)
    {
        // Register AI service
        switch (appConfig.RagConfig.AIChatService)
        {
            case "AzureOpenAI":
                kernelBuilder.AddAzureOpenAIChatCompletion(
                    appConfig.AzureOpenAIConfig.ChatDeploymentName,
                    appConfig.AzureOpenAIConfig.Endpoint,
                    new DefaultAzureCredential());
                break;
            case "OpenAI":
                kernelBuilder.AddOpenAIChatCompletion(
                    appConfig.OpenAIConfig.ChatModelId,
                    appConfig.OpenAIConfig.ApiKey,
                    appConfig.OpenAIConfig.OrgId);
                break;
            default:
                throw new NotSupportedException($"AI Chat Service type '{appConfig.RagConfig.AIChatService}' is not supported.");
        }

        // Register vector store
        switch (appConfig.RagConfig.VectorStore)
        {
            case "InMemory":
                kernelBuilder.AddInMemoryVectorStoreRecordCollection<string, TextSnippet<string>>(
                    appConfig.RagConfig.CollectionName);
                break;
            default:
                throw new NotSupportedException($"Vector store type '{appConfig.RagConfig.VectorStore}' is not supported.");
        }
    }
}
