// Copyright (c) Microsoft. All rights reserved.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace FunctionCalling;

/// <summary>
/// These samples demonstrate how to use function policies.
/// </summary>
public class FunctionCalling_FunctionPolicies(ITestOutputHelper output) : BaseTest(output)
{
    [Fact]
    /// <summary>
    /// This sample demonstrates how to specify an underscore as the plugin-function name separator.
    /// </summary>
    public async Task UseUnderscoreAsFunctionNameSeparatorAsync()
    {
        // Create an instance of the kernel builder.
        IKernelBuilder builder = Kernel.CreateBuilder();

        // Register the OpenAI chat completion service and specify a custom function name policy that uses an underscore as the separator.
        builder.AddOpenAIChatCompletion(
            TestConfiguration.OpenAI.ChatModelId,
            TestConfiguration.OpenAI.ApiKey,
            functionPolicies: new() { FunctionNamePolicy = new() { FunctionNameSeparator = "_" } });

        // Build the kernel and import the UtilsPlugin.
        Kernel kernel = builder.Build();
        kernel.ImportPluginFromType<UtilsPlugin>();

        IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

        ChatMessageContent result = await chatCompletionService.GetChatMessageContentAsync(
            "Given the current time of day and weather, what is the likely color of the sky in Boston?",
            new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() },
            kernel);

        // Assert
        Console.WriteLine(result);

        // Expected output: "The likely color of the sky in Boston is gray due to the current rainy weather."
    }

    [Fact]
    /// <summary>
    /// This sample demonstrates how to use the function name only for function advertising and function lookup.
    /// The plugin name is not used at all to avoid cases where AI models hallucinate "_" as the plugin name - function name separator instead of "-".
    /// </summary>
    public async Task UseOnlyFunctionNamesAndNotUsePluginNameForFunctionFqnAsync()
    {
        // Create an instance of the kernel builder.
        IKernelBuilder builder = Kernel.CreateBuilder();

        // Register the OpenAI chat completion service and specify a custom function name policy that uses an underscore as the separator.
        builder.AddOpenAIChatCompletion(
            TestConfiguration.OpenAI.ChatModelId,
            TestConfiguration.OpenAI.ApiKey,
            functionPolicies: new() { FunctionNamePolicy = new() { UseFunctionNameOnly = true } });

        // Build the kernel and import the UtilsPlugin.
        Kernel kernel = builder.Build();
        kernel.ImportPluginFromType<UtilsPlugin>();

        IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

        ChatMessageContent result = await chatCompletionService.GetChatMessageContentAsync(
            "Given the current time of day and weather, what is the likely color of the sky in Boston?",
            new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() },
            kernel);

        // Assert
        Console.WriteLine(result);

        // Expected output: "The likely color of the sky in Boston is gray due to the current rainy weather."
    }

    public sealed class UtilsPlugin
    {
        [KernelFunction]
        public string GetCurrentUtcDateTime()
        {
            return DateTime.UtcNow.ToString("R");
        }

        [KernelFunction]
        public string GetWeatherForCity(string cityName, string currentDateTime)
        {
            return cityName switch
            {
                "Boston" => "61 and rainy",
                "London" => "55 and cloudy",
                "Miami" => "80 and sunny",
                "Paris" => "60 and rainy",
                "Tokyo" => "50 and sunny",
                "Sydney" => "75 and sunny",
                "Tel Aviv" => "80 and sunny",
                _ => "31 and snowing",
            };
        }
    }
}
