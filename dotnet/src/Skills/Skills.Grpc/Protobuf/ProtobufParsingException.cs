// Copyright (c) Microsoft. All rights reserved.

using System;

namespace Microsoft.SemanticKernel.Skills.Grpc.Protobuf;

/// <summary>
/// Exception to be throw if a .proto file parsing has failed.
/// </summary>
[Obsolete("This class is deprecated and will be removed in one of the upcoming SK SDK versions. Instead, please expect an SKException or one of its derivatives.")]
public class ProtobufParsingException : Exception
{
    /// <summary>
    /// Creates an instance of a <see cref="ProtobufParsingException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    internal ProtobufParsingException(string message) : base(message)
    {
    }

    /// <summary>
    /// Creates an instance of a <see cref="ProtobufParsingException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    internal ProtobufParsingException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Creates an instance of a <see cref="ProtobufParsingException"/> class.
    /// </summary>
    internal ProtobufParsingException()
    {
    }
}
