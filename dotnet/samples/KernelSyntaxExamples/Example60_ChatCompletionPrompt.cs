// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using RepoUtils;

/**
 * The following example shows how to use Semantic Kernel with OpenAI Chat Completion API
 */
public static class Example60_ChatCompletionPrompt
{
    public static async Task RunAsync()
    {
        await OpenAIChatSampleAsync();
    }

    private static async Task OpenAIChatSampleAsync()
    {
        var folder = RepoFiles.SamplePluginsPath();

        var kernel = new KernelBuilder()
            .WithLoggerFactory(ConsoleLogger.LoggerFactory)
            .WithOpenAIChatCompletionService(TestConfiguration.OpenAI.ChatModelId, TestConfiguration.OpenAI.ApiKey)
            .Build();

        var functions = kernel.ImportSemanticFunctionsFromDirectory(folder, new[] { "ChatPlugin" });

        var kernelResult = await kernel.RunAsync("buy house", functions["ChatWithRoles"]);

        var functionResult = kernelResult.FunctionResults.FirstOrDefault();

        var result = functionResult.GetValue<string>();

        Console.WriteLine(result);
    }
}
