// Copyright (c) Microsoft. All rights reserved.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.SemanticKernel.SkillDefinition;

#pragma warning disable CS0618 // Temporarily suppressing obsoletion warnings until the code is refactored to no longer use obsolete exception types.

[DebuggerDisplay("Count = 0")]
internal sealed class NullReadOnlySkillCollection : IReadOnlySkillCollection
{
    public static readonly NullReadOnlySkillCollection Instance = new();

    public ISKFunction GetFunction(string functionName)
    {
        return ThrowFunctionNotAvailable(functionName);
    }

    public ISKFunction GetFunction(string skillName, string functionName)
    {
        return ThrowFunctionNotAvailable(skillName, functionName);
    }

    public bool TryGetFunction(string functionName, [NotNullWhen(true)] out ISKFunction? availableFunction)
    {
        availableFunction = null;
        return false;
    }

    public bool TryGetFunction(string skillName, string functionName, [NotNullWhen(true)] out ISKFunction? availableFunction)
    {
        availableFunction = null;
        return false;
    }

    public FunctionsView GetFunctionsView(bool includeSemantic = true, bool includeNative = true)
    {
        return new();
    }

    private NullReadOnlySkillCollection()
    {
    }

    [DoesNotReturn]
    private static ISKFunction ThrowFunctionNotAvailable(string skillName, string functionName)
    {
        throw new KernelException(
            KernelException.ErrorCodes.FunctionNotAvailable,
            $"Function not available: {skillName}.{functionName}");
    }

    [DoesNotReturn]
    private static ISKFunction ThrowFunctionNotAvailable(string functionName)
    {
        throw new KernelException(
            KernelException.ErrorCodes.FunctionNotAvailable,
            $"Function not available: {functionName}");
    }
}
