﻿// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Microsoft.SemanticKernel;
using Xunit;

namespace SemanticKernel.UnitTests.AI.FunctionChoiceBehaviors;

/// <summary>
/// Unit tests for <see cref="RequiredFunctionChoiceBehavior"/>
/// </summary>
public sealed class RequiredFunctionChoiceBehaviorTests
{
    private readonly Kernel _kernel;

    public RequiredFunctionChoiceBehaviorTests()
    {
        this._kernel = new Kernel();
    }

    [Fact]
    public void ItShouldAdvertiseAllKernelFunctions()
    {
        // Arrange
        var plugin = GetTestPlugin();
        this._kernel.Plugins.Add(plugin);

        // Act
        var choiceBehavior = new RequiredFunctionChoiceBehavior();

        var config = choiceBehavior.GetConfiguration(new([]) { Kernel = this._kernel });

        // Assert
        Assert.NotNull(config);

        Assert.NotNull(config.Functions);
        Assert.Equal(3, config.Functions.Count);
        Assert.Contains(config.Functions, f => f.Name == "Function1");
        Assert.Contains(config.Functions, f => f.Name == "Function2");
        Assert.Contains(config.Functions, f => f.Name == "Function3");
    }

    [Fact]
    public void ItShouldAdvertiseOnlyFunctionsSuppliedViaConstructor()
    {
        // Arrange
        var plugin = GetTestPlugin();
        this._kernel.Plugins.Add(plugin);

        // Act
        var choiceBehavior = new RequiredFunctionChoiceBehavior(functions: [plugin.ElementAt(0), plugin.ElementAt(1)]);

        var config = choiceBehavior.GetConfiguration(new([]) { Kernel = this._kernel });

        // Assert
        Assert.NotNull(config);

        Assert.NotNull(config.Functions);
        Assert.Equal(2, config.Functions.Count);
        Assert.Contains(config.Functions, f => f.Name == "Function1");
        Assert.Contains(config.Functions, f => f.Name == "Function2");
    }

    //[Fact]
    //public void ItShouldAdvertiseOnlyFunctionsSuppliedInFunctionsProperty()
    //{
    //    // Arrange
    //    var plugin = GetTestPlugin();
    //    this._kernel.Plugins.Add(plugin);

    //    // Act
    //    var choiceBehavior = new RequiredFunctionChoiceBehavior()
    //    {
    //        Functions = ["MyPlugin.Function1", "MyPlugin.Function2"]
    //    };

    //    var config = choiceBehavior.GetConfiguration(new() { Kernel = this._kernel });

    //    // Assert
    //    Assert.NotNull(config);

    //    Assert.NotNull(config.Functions);
    //    Assert.Equal(2, config.Functions.Count);
    //    Assert.Contains(config.Functions, f => f.Name == "Function1");
    //    Assert.Contains(config.Functions, f => f.Name == "Function2");
    //}

    [Fact]
    public void ItShouldAdvertiseOnlyFunctionsSuppliedViaConstructorForManualInvocation()
    {
        // Arrange
        var plugin = GetTestPlugin();

        // Act
        var choiceBehavior = new RequiredFunctionChoiceBehavior([plugin.ElementAt(0), plugin.ElementAt(1)], autoInvoke: false);

        var config = choiceBehavior.GetConfiguration(new([]) { Kernel = this._kernel });

        // Assert
        Assert.NotNull(config);

        Assert.NotNull(config.Functions);
        Assert.Equal(2, config.Functions.Count);
        Assert.Contains(config.Functions, f => f.Name == "Function1");
        Assert.Contains(config.Functions, f => f.Name == "Function2");
    }

    [Fact]
    public void ItShouldAdvertiseAllKernelFunctionsForManualInvocation()
    {
        // Arrange
        var plugin = GetTestPlugin();
        this._kernel.Plugins.Add(plugin);

        // Act
        var choiceBehavior = new RequiredFunctionChoiceBehavior(autoInvoke: false);

        var config = choiceBehavior.GetConfiguration(new([]) { Kernel = this._kernel });

        // Assert
        Assert.NotNull(config);

        Assert.NotNull(config.Functions);
        Assert.Equal(3, config.Functions.Count);
        Assert.Contains(config.Functions, f => f.Name == "Function1");
        Assert.Contains(config.Functions, f => f.Name == "Function2");
        Assert.Contains(config.Functions, f => f.Name == "Function3");
    }

    [Fact]
    public void ItShouldAllowAutoInvocationByDefault()
    {
        // Arrange
        var plugin = GetTestPlugin();
        this._kernel.Plugins.Add(plugin);

        // Act
        var choiceBehavior = new RequiredFunctionChoiceBehavior();

        var config = choiceBehavior.GetConfiguration(new([]) { Kernel = this._kernel });

        // Assert
        Assert.NotNull(config);
        Assert.True(config.AutoInvoke);
    }

    [Fact]
    public void ItShouldAllowManualInvocation()
    {
        // Arrange
        var plugin = GetTestPlugin();
        this._kernel.Plugins.Add(plugin);

        // Act
        var choiceBehavior = new RequiredFunctionChoiceBehavior(autoInvoke: false);

        var config = choiceBehavior.GetConfiguration(new([]) { Kernel = this._kernel });

        // Assert
        Assert.NotNull(config);
        Assert.False(config.AutoInvoke);
    }

    //[Fact]
    //public void ItShouldInitializeFunctionPropertyByFunctionsPassedViaConstructor()
    //{
    //    // Arrange
    //    var plugin = GetTestPlugin();
    //    this._kernel.Plugins.Add(plugin);

    //    // Act
    //    var choiceBehavior = new RequiredFunctionChoiceBehavior(functions: [plugin.ElementAt(0), plugin.ElementAt(1)]);

    //    // Assert
    //    Assert.NotNull(choiceBehavior.Functions);
    //    Assert.Equal(2, choiceBehavior.Functions.Count);

    //    Assert.Equal("MyPlugin.Function1", choiceBehavior.Functions.ElementAt(0));
    //    Assert.Equal("MyPlugin.Function2", choiceBehavior.Functions.ElementAt(1));
    //}

    [Fact]
    public void ItShouldThrowExceptionIfAutoInvocationRequestedButNoKernelIsProvided()
    {
        // Arrange
        var plugin = GetTestPlugin();
        this._kernel.Plugins.Add(plugin);

        var choiceBehavior = new RequiredFunctionChoiceBehavior();

        // Act
        var exception = Assert.Throws<KernelException>(() =>
        {
            choiceBehavior.GetConfiguration(new([]) { Kernel = null });
        });

        Assert.Equal("Auto-invocation is not supported when no kernel is provided.", exception.Message);
    }

    [Fact]
    public void ItShouldThrowExceptionIfAutoInvocationRequestedAndFunctionIsNotRegisteredInKernel()
    {
        // Arrange
        var plugin = GetTestPlugin();

        var choiceBehavior = new RequiredFunctionChoiceBehavior(functions: [plugin.ElementAt(0)]);

        // Act
        var exception = Assert.Throws<KernelException>(() =>
        {
            choiceBehavior.GetConfiguration(new([]) { Kernel = this._kernel });
        });

        Assert.Equal("The specified function MyPlugin.Function1 is not available in the kernel.", exception.Message);
    }

    //[Fact]
    //public void ItShouldThrowExceptionIfNoFunctionFoundAndManualInvocationIsRequested()
    //{
    //    // Arrange
    //    var plugin = GetTestPlugin();
    //    this._kernel.Plugins.Add(plugin);

    //    var choiceBehavior = new RequiredFunctionChoiceBehavior(autoInvoke: false)
    //    {
    //        Functions = ["MyPlugin.NonKernelFunction"]
    //    };

    //    // Act
    //    var exception = Assert.Throws<KernelException>(() =>
    //    {
    //        choiceBehavior.GetConfiguration(new() { Kernel = this._kernel });
    //    });

    //    Assert.Equal("The specified function MyPlugin.NonKernelFunction was not found.", exception.Message);
    //}

    [Fact]
    public void ItShouldReturnNoFunctionAsSpecifiedByFunctionsSelector()
    {
        // Arrange
        var plugin = GetTestPlugin();
        this._kernel.Plugins.Add(plugin);

        static IReadOnlyList<KernelFunction>? FunctionsSelector(FunctionChoiceBehaviorFunctionsSelectorContext context)
        {
            return [];
        }

        // Act
        var choiceBehavior = new RequiredFunctionChoiceBehavior(autoInvoke: true, functionsSelector: FunctionsSelector);

        var config = choiceBehavior.GetConfiguration(new([]) { Kernel = this._kernel });

        // Assert
        Assert.NotNull(config.Functions);
        Assert.Empty(config.Functions);
    }

    [Fact]
    public void ItShouldReturnFunctionsAsSpecifiedByFunctionsSelector()
    {
        // Arrange
        var plugin = GetTestPlugin();
        this._kernel.Plugins.Add(plugin);

        static IReadOnlyList<KernelFunction>? FunctionsSelector(FunctionChoiceBehaviorFunctionsSelectorContext context)
        {
            return context.Functions!.Where(f => f.Name == "Function1").ToList();
        }

        // Act
        var choiceBehavior = new RequiredFunctionChoiceBehavior(autoInvoke: true, functionsSelector: FunctionsSelector);

        var config = choiceBehavior.GetConfiguration(new([]) { Kernel = this._kernel });

        // Assert
        Assert.NotNull(config?.Functions);
        Assert.Single(config.Functions);
        Assert.Equal("Function1", config.Functions[0].Name);
    }

    private static KernelPlugin GetTestPlugin()
    {
        var function1 = KernelFunctionFactory.CreateFromMethod(() => { }, "Function1");
        var function2 = KernelFunctionFactory.CreateFromMethod(() => { }, "Function2");
        var function3 = KernelFunctionFactory.CreateFromMethod(() => { }, "Function3");

        return KernelPluginFactory.CreateFromFunctions("MyPlugin", [function1, function2, function3]);
    }
}
