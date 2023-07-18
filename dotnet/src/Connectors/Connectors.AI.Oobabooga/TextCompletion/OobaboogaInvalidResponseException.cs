// Copyright (c) Microsoft. All rights reserved.

using System;
using Microsoft.SemanticKernel.AI;

namespace Microsoft.SemanticKernel.Connectors.AI.Oobabooga.TextCompletion;

#pragma warning disable RCS1194 // Implement exception constructors.
[Obsolete("This class is deprecated and will be removed in one of the upcoming SK SDK versions. Instead, please expect an SKException or one of its derivatives.")]
internal sealed class OobaboogaInvalidResponseException<T> : AIException
{
    public T? ResponseData { get; }

    public OobaboogaInvalidResponseException(T? responseData, string? message = null) : base(ErrorCodes.InvalidResponseContent, message)
    {
        this.ResponseData = responseData;
    }
}
