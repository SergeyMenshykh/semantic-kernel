﻿// Copyright (c) Microsoft. All rights reserved.

using System.Linq;
using Microsoft.SemanticKernel;
using Xunit;

namespace SemanticKernel.UnitTests.Functions;

/// <summary>
/// Unit tests for <see cref="FunctionChoiceBehavior"/>
/// </summary>
public sealed class FunctionChoiceBehaviorTests
{
    private readonly Kernel _kernel;

    public FunctionChoiceBehaviorTests()
    {
        this._kernel = new Kernel();
    }

    [Fact]
    public void AutoFunctionChoiceShouldBeUsed()
    {
        // Act
        var choiceBehavior = FunctionChoiceBehavior.Auto();

        // Assert
        Assert.IsType<AutoFunctionChoiceBehavior>(choiceBehavior);
    }

    //[Fact]
    //public void RequiredFunctionChoiceShouldBeUsed()
    //{
    //    // Act
    //    var choiceBehavior = FunctionChoiceBehavior.Required();

    //    // Assert
    //    Assert.IsType<RequiredFunctionChoiceBehavior>(choiceBehavior);
    //}

    //[Fact]
    //public void NoneFunctionChoiceShouldBeUsed()
    //{
    //    // Act
    //    var choiceBehavior = FunctionChoiceBehavior.None();

    //    // Assert
    //    Assert.IsType<NoneFunctionChoiceBehavior>(choiceBehavior);
    //}

    [Fact]
    public void AutoFunctionChoiceShouldAdvertiseKernelFunctions()
    {
        // Arrange
        var plugin = GetTestPlugin();
        this._kernel.Plugins.Add(plugin);

        // Act
        var choiceBehavior = FunctionChoiceBehavior.Auto(functions: null);

        var config = choiceBehavior.GetConfiguration(new() { Kernel = this._kernel });

        // Assert
        Assert.NotNull(config);

        Assert.NotNull(config.Functions);
        Assert.Equal(3, config.Functions.Count);
        Assert.Contains(config.Functions, f => f.Name == "Function1");
        Assert.Contains(config.Functions, f => f.Name == "Function2");
        Assert.Contains(config.Functions, f => f.Name == "Function3");
    }

    [Fact]
    public void AutoFunctionChoiceShouldAdvertiseProvidedFunctions()
    {
        // Arrange
        var plugin = GetTestPlugin();
        this._kernel.Plugins.Add(plugin);

        // Act
        var choiceBehavior = FunctionChoiceBehavior.Auto(functions: [plugin.ElementAt(0), plugin.ElementAt(1)]);

        var config = choiceBehavior.GetConfiguration(new() { Kernel = this._kernel });

        // Assert
        Assert.NotNull(config);

        Assert.NotNull(config.Functions);
        Assert.Equal(2, config.Functions.Count);
        Assert.Contains(config.Functions, f => f.Name == "Function1");
        Assert.Contains(config.Functions, f => f.Name == "Function2");
    }

    [Fact]
    public void AutoFunctionChoiceShouldAllowAutoInvocation()
    {
        // Arrange
        var plugin = GetTestPlugin();
        this._kernel.Plugins.Add(plugin);

        // Act
        var choiceBehavior = FunctionChoiceBehavior.Auto(autoInvoke: true);

        var config = choiceBehavior.GetConfiguration(new() { Kernel = this._kernel });

        // Assert
        Assert.NotNull(config);
        Assert.True(config.AutoInvoke);
    }

    [Fact]
    public void AutoFunctionChoiceShouldAllowManualInvocation()
    {
        // Arrange
        var plugin = GetTestPlugin();
        this._kernel.Plugins.Add(plugin);

        // Act
        var choiceBehavior = FunctionChoiceBehavior.Auto(autoInvoke: false);

        var config = choiceBehavior.GetConfiguration(new() { Kernel = this._kernel });

        // Assert
        Assert.NotNull(config);
        Assert.False(config.AutoInvoke);
    }

    //[Fact]
    //public void RequiredFunctionChoiceShouldAdvertiseKernelFunctions()
    //{
    //    // Arrange
    //    var plugin = GetTestPlugin();
    //    this._kernel.Plugins.Add(plugin);

    //    // Act
    //    var choiceBehavior = FunctionChoiceBehavior.Required(functions: null);

    //    var config = choiceBehavior.GetConfiguration(new() { Kernel = this._kernel });

    //    // Assert
    //    Assert.NotNull(config);

    //    Assert.NotNull(config.Functions);
    //    Assert.Equal(3, config.Functions.Count);
    //    Assert.Contains(config.Functions, f => f.Name == "Function1");
    //    Assert.Contains(config.Functions, f => f.Name == "Function2");
    //    Assert.Contains(config.Functions, f => f.Name == "Function3");
    //}

    //[Fact]
    //public void RequiredFunctionChoiceShouldAdvertiseProvidedFunctions()
    //{
    //    // Arrange
    //    var plugin = GetTestPlugin();
    //    this._kernel.Plugins.Add(plugin);

    //    // Act
    //    var choiceBehavior = FunctionChoiceBehavior.Required(functions: [plugin.ElementAt(0), plugin.ElementAt(1)]);

    //    var config = choiceBehavior.GetConfiguration(new() { Kernel = this._kernel });

    //    // Assert
    //    Assert.NotNull(config);

    //    Assert.NotNull(config.Functions);
    //    Assert.Equal(2, config.Functions.Count);
    //    Assert.Contains(config.Functions, f => f.Name == "Function1");
    //    Assert.Contains(config.Functions, f => f.Name == "Function2");
    //}

    //[Fact]
    //public void RequiredFunctionChoiceShouldAllowAutoInvocation()
    //{
    //    // Arrange
    //    var plugin = GetTestPlugin();
    //    this._kernel.Plugins.Add(plugin);

    //    // Act
    //    var choiceBehavior = FunctionChoiceBehavior.Required(options: new() { AutoInvoke = true });

    //    var config = choiceBehavior.GetConfiguration(new() { Kernel = this._kernel });

    //    // Assert
    //    Assert.NotNull(config);
    //    Assert.True(config.Options.AutoInvoke);
    //}

    //[Fact]
    //public void RequiredFunctionChoiceShouldAllowManualInvocation()
    //{
    //    // Arrange
    //    var plugin = GetTestPlugin();
    //    this._kernel.Plugins.Add(plugin);

    //    // Act
    //    var choiceBehavior = FunctionChoiceBehavior.Required(options: new() { AutoInvoke = false });

    //    var config = choiceBehavior.GetConfiguration(new() { Kernel = this._kernel });

    //    // Assert
    //    Assert.NotNull(config);
    //    Assert.False(config.Options.AutoInvoke);
    //}

    //[Fact]
    //public void NoneFunctionChoiceShouldAdvertiseProvidedFunctions()
    //{
    //    // Arrange
    //    var plugin = GetTestPlugin();

    //    // Act
    //    var choiceBehavior = FunctionChoiceBehavior.None([plugin.ElementAt(0), plugin.ElementAt(2)]);

    //    var config = choiceBehavior.GetConfiguration(new() { Kernel = this._kernel });

    //    // Assert
    //    Assert.NotNull(config);

    //    Assert.NotNull(config.Functions);
    //    Assert.Equal(2, config.Functions.Count);
    //    Assert.Contains(config.Functions, f => f.Name == "Function1");
    //    Assert.Contains(config.Functions, f => f.Name == "Function3");
    //}

    //[Fact]
    //public void NoneFunctionChoiceShouldAdvertiseAllKernelFunctions()
    //{
    //    // Arrange
    //    var plugin = GetTestPlugin();
    //    this._kernel.Plugins.Add(plugin);

    //    // Act
    //    var choiceBehavior = FunctionChoiceBehavior.None();

    //    var config = choiceBehavior.GetConfiguration(new() { Kernel = this._kernel });

    //    // Assert
    //    Assert.NotNull(config);

    //    Assert.NotNull(config.Functions);
    //    Assert.Equal(3, config.Functions.Count);
    //    Assert.Contains(config.Functions, f => f.Name == "Function1");
    //    Assert.Contains(config.Functions, f => f.Name == "Function2");
    //    Assert.Contains(config.Functions, f => f.Name == "Function3");
    //}

    private static KernelPlugin GetTestPlugin()
    {
        var function1 = KernelFunctionFactory.CreateFromMethod(() => { }, "Function1");
        var function2 = KernelFunctionFactory.CreateFromMethod(() => { }, "Function2");
        var function3 = KernelFunctionFactory.CreateFromMethod(() => { }, "Function3");

        return KernelPluginFactory.CreateFromFunctions("MyPlugin", [function1, function2, function3]);
    }
}
