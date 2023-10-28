// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Text;

namespace Microsoft.SemanticKernel.Connectors.AI.OpenAI.ChatCompletion;

/// <summary>
/// OpenAI Chat content
/// See https://platform.openai.com/docs/guides/chat for details
/// </summary>
public class OpenAIChatHistory : ChatHistory
{
    /// <summary>
    /// Create a new and empty chat history
    /// </summary>
    /// <param name="assistantInstructions">Optional instructions for the assistant</param>
    public OpenAIChatHistory(string? assistantInstructions = null)
    {
        if (!assistantInstructions.IsNullOrWhitespace())
        {
            var messages = ExtractMessages(assistantInstructions);
            if (messages.Count == 0)
            {
                this.AddSystemMessage(assistantInstructions);
            }

            this.AddRange(messages);
        }
    }

    private static List<ChatMessage> ExtractMessages(string prompts)
    {
        var messages = new List<ChatMessage>();

        var pattern = @"<message role=""(.*?)""[^>]*>\s*(.*?)\s*</message>";

        var matches = Regex.Matches(prompts, pattern, RegexOptions.Singleline);

        foreach (Match match in matches)
        {
            var role = match.Groups[1].Value.Trim();
            var text = match.Groups[2].Value.Trim();

            messages.Add(new ChatMessage(new AuthorRole(role), text));
        }

        return messages;
    }
}
