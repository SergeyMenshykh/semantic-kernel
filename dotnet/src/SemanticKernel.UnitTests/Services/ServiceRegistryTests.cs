// Copyright (c) Microsoft. All rights reserved.

using Microsoft.SemanticKernel.Services;
using Xunit;

namespace SemanticKernel.UnitTests.Services;

/// <summary>
/// Unit tests of <see cref="AIServiceCollection"/>.
/// </summary>
public class ServiceRegistryTests
{
    [Fact]
    public void ItCanSetAndRetrieveServiceInstance()
    {
        // Arrange
        var services = new AIServiceCollection();
        var service = new TestService();

        // Act
        services.SetService<ITestService>(service);
        var provider = services.Build();
        var result = provider.GetService<ITestService>();

        // Assert
        Assert.Same(service, result);
    }

    [Fact]
    public void ItCanSetAndRetrieveServiceInstanceWithName()
    {
        // Arrange
        var services = new AIServiceCollection();
        var service1 = new TestService();
        var service2 = new TestService();

        // Act
        services.SetService<ITestService>("foo", service1);
        services.SetService<ITestService>("bar", service2);
        var provider = services.Build();

        // Assert
        Assert.Same(service1, provider.GetService<ITestService>("foo"));
        Assert.Same(service2, provider.GetService<ITestService>("bar"));
    }

    [Fact]
    public void ItCanSetAndRetrieveServiceFactory()
    {
        // Arrange
        var services = new AIServiceCollection();
        var service = new TestService();

        // Act
        services.SetService<ITestService>(() => service);
        var provider = services.Build();

        // Assert
        Assert.Same(service, provider.GetService<ITestService>());
    }

    [Fact]
    public void ItCanSetAndRetrieveServiceFactoryWithName()
    {
        // Arrange
        var services = new AIServiceCollection();
        var service1 = new TestService();
        var service2 = new TestService();

        // Act
        services.SetService<ITestService>("foo", () => service1);
        services.SetService<ITestService>("bar", () => service2);
        var provider = services.Build();

        // Assert
        Assert.Same(service1, provider.GetService<ITestService>("foo"));
        Assert.Same(service2, provider.GetService<ITestService>("bar"));
    }

    [Fact]
    public void ItCanSetAndRetrieveServiceFactoryWithServiceProvider()
    {
        // Arrange
        var services = new AIServiceCollection();
        var service = new TestService();

        // Act
        services.SetService<ITestService>(() => service);
        var provider = services.Build();

        // Assert
        Assert.Same(service, provider.GetService<ITestService>());
    }

    [Fact]
    public void ItCanSetAndRetrieveServiceFactoryWithServiceProviderAndName()
    {
        // Arrange
        var services = new AIServiceCollection();
        var service1 = new TestService();
        var service2 = new TestService();

        // Act
        services.SetService<ITestService>("foo", () => service1);
        services.SetService<ITestService>("bar", () => service2);
        var provider = services.Build();

        // Assert
        Assert.Same(service1, provider.GetService<ITestService>("foo"));
        Assert.Same(service2, provider.GetService<ITestService>("bar"));
    }

    [Fact]
    public void ItCanSetDefaultService()
    {
        // Arrange
        var services = new AIServiceCollection();
        var service1 = new TestService();
        var service2 = new TestService();

        // Act
        services.SetService<ITestService>("foo", service1);
        services.SetService<ITestService>("bar", service2, setAsDefault: true);
        var provider = services.Build();

        // Assert
        Assert.Same(service2, provider.GetService<ITestService>());
    }

    [Fact]
    public void ItCanSetDefaultServiceFactory()
    {
        // Arrange
        var services = new AIServiceCollection();
        var service1 = new TestService();
        var service2 = new TestService();

        // Act
        services.SetService<ITestService>("foo", () => service1);
        services.SetService<ITestService>("bar", () => service2, setAsDefault: true);
        var provider = services.Build();

        // Assert
        Assert.Same(service2, provider.GetService<ITestService>());
    }

    [Fact]
    public void ItCanSetDefaultServiceFactoryWithServiceProvider()
    {
        // Arrange
        var services = new AIServiceCollection();
        var service1 = new TestService();
        var service2 = new TestService();

        // Act
        services.SetService<ITestService>("foo", () => service1);
        services.SetService<ITestService>("bar", () => service2, setAsDefault: true);
        var provider = services.Build();

        // Assert
        Assert.Same(service2, provider.GetService<ITestService>());
    }

    public interface ITestService { }

    // A test service implementation
    private sealed class TestService : ITestService
    {
    }
}
