﻿// Copyright (c) Microsoft. All rights reserved.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace FunctionCalling;

/// <summary>
/// These samples demonstrate how to use a custom function name policy to specify Fully Qualified Name (FQN) separator and parse function FQN.
/// </summary>
public class FunctionCalling_FunctionNamePolicy(ITestOutputHelper output) : BaseTest(output)
{
    /// <summary>
    /// This sample demonstrates how to use an underscore as the function FQN separator.
    /// This can be useful if the default separator, which is a hyphen ("-"), is not suitable for your needs.
    /// For example, if your plugin name is "foo" and your function name is "bar," the function FQN will be "foo_bar" instead of "foo-bar".
    /// </summary>
    [Fact]
    public async Task UseUnderscoreAsFunctionFqnSeparatorAsync()
    {
        // Create an instance of the kernel builder.
        IKernelBuilder builder = Kernel.CreateBuilder();

        // Register the OpenAI chat completion service and specify a custom function name policy that uses an underscore as the separator.
        builder.AddOpenAIChatCompletion(
            TestConfiguration.OpenAI.ChatModelId,
            TestConfiguration.OpenAI.ApiKey,
            functionNamePolicy: FunctionNamePolicy.Custom("_"));

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

    /// <summary>
    /// This sample demonstrates how to prevent the plugin name from being included in the FQN of a function.
    /// Instead, only the function name will be used for the FQN.
    /// For example, if your plugin name is "foo" and your function name is "bar" the function's FQN will be "bar" instead of "foo-bar."
    /// </summary>
    [Fact]
    public async Task UseOnlyFunctionNameAndDoNotUsePluginNameForFqnAsync()
    {
        // Create an instance of the kernel builder.
        IKernelBuilder builder = Kernel.CreateBuilder();

        // Register the OpenAI chat completion service and specify a custom function name policy that uses an underscore as the separator.
        builder.AddOpenAIChatCompletion(
            TestConfiguration.OpenAI.ChatModelId,
            TestConfiguration.OpenAI.ApiKey,
            functionNamePolicy: FunctionNamePolicy.FunctionNameOnly);

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

    /// <summary>
    /// This sample demonstrates how to use a custom function FQN parser
    /// to handle a hallucinated function name generated by AI model.
    /// It addresses the scenario where the AI model calls the function using a different separator
    /// than the one used in the function's original declaration.
    /// For example, you function is advertised as "foo-bar" but the AI model calls it as "foo_bar"
    /// </summary>
    [Fact]
    public async Task RecoverHallucinatedFunctionNameAsync()
    {
        // Create an instance of the kernel builder.
        IKernelBuilder builder = Kernel.CreateBuilder();

        // Define a custom function FQN parser that can handle a hallucinated function name.
        static (string? PluginName, string FunctioName) ParseFunctionFqn(ParseFunctionFqnContext context)
        {
            // Try to use use hyphen as separator.
            var parts = context.FunctionFqn.Split('-');
            if (parts.Length == 2)
            {
                // Check if the function registered in the kernel
                if (context.Kernel is { } kernel && kernel.Plugins.TryGetFunction(parts[0], parts[1], out _))
                {
                    return (parts[0], parts[1]);
                }
            }

            // If hyphen is not found, try to use underscore as separator. This approach presumes the underscore symbol is not used in function name.  
            parts = context.FunctionFqn.Split('_');
            if (parts.Length == 2)
            {
                // Check if the function registered in the kernel
                if (context.Kernel is { } kernel && kernel.Plugins.TryGetFunction(parts[0], parts[1], out _))
                {
                    return (parts[0], parts[1]);
                }
            }

            // If no separator is found, return the function name as is.
            return (null, context.FunctionFqn);
        }

        // Register the OpenAI chat completion service and specify a custom function name policy that uses an underscore as the separator.
        builder.AddOpenAIChatCompletion(
            TestConfiguration.OpenAI.ChatModelId,
            TestConfiguration.OpenAI.ApiKey,
            functionNamePolicy: FunctionNamePolicy.Custom("-", ParseFunctionFqn));

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