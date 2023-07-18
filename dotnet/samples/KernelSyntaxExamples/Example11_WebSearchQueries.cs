// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Skills.Web;
using RepoUtils;

#pragma warning disable CS0618 // Temporarily suppressing obsoletion warnings until the code is refactored to throw exceptions.

// ReSharper disable once InconsistentNaming
public static class Example11_WebSearchQueries
{
    public static async Task RunAsync()
    {
        Console.WriteLine("======== WebSearchQueries ========");

        IKernel kernel = Kernel.Builder.WithLogger(ConsoleLogger.Log).Build();

        // Load native skills
        var skill = new SearchUrlSkill();
        var bing = kernel.ImportSkill(skill, "search");

        // Run
        var ask = "What's the tallest building in Europe?";
        var result = await kernel.RunAsync(
            ask,
            bing["BingSearchUrl"]
        );

        Console.WriteLine(ask + "\n");
        Console.WriteLine(result);
    }
}
