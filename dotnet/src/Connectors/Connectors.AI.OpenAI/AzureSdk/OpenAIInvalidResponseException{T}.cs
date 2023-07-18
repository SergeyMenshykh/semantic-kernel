// Copyright (c) Microsoft. All rights reserved.

using System;
using Microsoft.SemanticKernel.AI;

#pragma warning disable RCS1194 // Implement exception constructors.

namespace Microsoft.SemanticKernel.Connectors.AI.OpenAI.AzureSdk;

[Obsolete("This class is deprecated and will be removed in one of the upcoming SK SDK versions. Instead, please expect an SKException or one of its derivatives.")]
internal sealed class OpenAIInvalidResponseException<T> : AIException
{
    public T? ResponseData { get; }

    public OpenAIInvalidResponseException(T? responseData, string? message = null) : base(ErrorCodes.InvalidResponseContent, message)
    {
        this.ResponseData = responseData;
    }
}
