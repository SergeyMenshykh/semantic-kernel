// Copyright (c) Microsoft. All rights reserved.

using System;

namespace Microsoft.SemanticKernel.Connectors.Memory.Chroma;

/// <summary>
/// Exception to identify issues in <see cref="ChromaMemoryStore"/> class.
/// </summary>
[Obsolete("This class is deprecated and will be removed in one of the upcoming SK SDK versions. Instead, please expect an SKException or one of its derivatives.")]
public class ChromaMemoryStoreException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChromaMemoryStoreException"/> class.
    /// </summary>
    public ChromaMemoryStoreException() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChromaMemoryStoreException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ChromaMemoryStoreException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChromaMemoryStoreException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">Instance of inner exception.</param>
    public ChromaMemoryStoreException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
