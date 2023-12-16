// Copyright (c) Microsoft. All rights reserved.

using Microsoft.SemanticKernel;
using System.Globalization;
using System;
using Xunit;
using System.Text.Json;

namespace SemanticKernel.UnitTests.PromptTemplate;
public class InputVariableTests
{
    [Fact]
    public void ItShouldBePossibleToUseManyDonNetTypeAsDefaultValue()
    {
        // Arrange  
        var culture = CultureInfo.InvariantCulture;

        // Atc & Assert
        Assert.Null(new InputVariable { Default = null }.Default);

        Assert.Equal("abc", new InputVariable { Default = "abc" }.Default);
        Assert.Equal((byte)10, new InputVariable { Default = (byte)10 }.Default);
        Assert.Equal((sbyte)10, new InputVariable { Default = (sbyte)10 }.Default);
        Assert.Equal((short)10, new InputVariable { Default = (short)10 }.Default);
        Assert.Equal((ushort)10, new InputVariable { Default = (ushort)10 }.Default);
        Assert.Equal((int)10, new InputVariable { Default = (int)10 }.Default);
        Assert.Equal((uint)10, new InputVariable { Default = (uint)10 }.Default);
        Assert.Equal((long)10, new InputVariable { Default = (long)10 }.Default);
        Assert.Equal((ulong)10, new InputVariable { Default = (ulong)10 }.Default);
        Assert.Equal((float)10, new InputVariable { Default = (float)10 }.Default);
        Assert.Equal((double)10, new InputVariable { Default = (double)10 }.Default);
        Assert.Equal((decimal)10, new InputVariable { Default = (decimal)10 }.Default);
        Assert.Equal((char)10, new InputVariable { Default = (char)10 }.Default);
        Assert.Equal(true, new InputVariable { Default = (bool)true }.Default);

        var dateTime = DateTime.ParseExact("06.12.2023 11:53:36", "dd.MM.yyyy HH:mm:ss", culture);
        Assert.Equal(dateTime, new InputVariable { Default = dateTime }.Default);

        var dateTimeOffset = DateTimeOffset.ParseExact("06.12.2023 11:53:36 +02:00", "dd.MM.yyyy HH:mm:ss zzz", culture);
        Assert.Equal(dateTimeOffset, new InputVariable { Default = dateTimeOffset }.Default);

        Assert.Equal(TimeSpan.FromHours(1), new InputVariable { Default = TimeSpan.FromHours(1) }.Default);

        Guid guid = Guid.NewGuid();
        Assert.Equal(guid, new InputVariable { Default = guid }.Default);

        Assert.Equal(DayOfWeek.Monday, new InputVariable { Default = DayOfWeek.Monday }.Default);
    }

    [Fact]
    public void ItShouldBePossibleToUseCustomTypeAsDefaultValue()
    {
        // Arrange
        var customType = new MyCustomType { Name = "abc" };

        // Atc & Assert
        Assert.Same(customType, new InputVariable { Default = customType }.Default);
    }

    [Fact]
    public void ItShouldBePossibleToDeserializeDefaultFromString()
    {
        // Arrange
        var json = "{\"default\":\"abc\"}";

        // Act
        var inputVariable = JsonSerializer.Deserialize<InputVariable>(json);

        // Assert
        Assert.Equal("abc", inputVariable?.Default);
    }

    [Fact]
    public void ItShouldBePossibleToDeserializeDefaultFromNumbers()
    {
        // long  
        Assert.Equal(int.MaxValue - 1, JsonSerializer.Deserialize<InputVariable>("{\"default\": 2147483646}")?.Default);

        // long  
        Assert.Equal(long.MaxValue - 1, JsonSerializer.Deserialize<InputVariable>("{\"default\": 9223372036854775806}")?.Default);

        // decimal  
        Assert.Equal(decimal.MaxValue - 1, JsonSerializer.Deserialize<InputVariable>("{\"default\": 79228162514264337593543950334}")?.Default);

        // double  
        Assert.Equal(double.MaxValue - 1, JsonSerializer.Deserialize<InputVariable>("{\"default\": 1.7976931348623157E+308}")?.Default);
    }

    [Fact]
    public void ItShouldBePossibleToDeserializeDefaultFromBoolean()
    {
        // true
        Assert.Equal(true, JsonSerializer.Deserialize<InputVariable>("{\"default\": true}")?.Default);

        // false
        Assert.Equal(false, JsonSerializer.Deserialize<InputVariable>("{\"default\": false}")?.Default);
    }

    [Fact]
    public void ItShouldBePossibleToDeserializeDefaultFromNull()
    {
        Assert.Null(JsonSerializer.Deserialize<InputVariable>("{\"default\": null}")?.Default);
    }

    [Fact]
    public void ItShouldNotBePossibleToDeserializeDefaultFromObject()
    {
        // Arrange
        var json = "{\"default\": {\"p1\": 2}}";

        // Act
        var result = JsonSerializer.Deserialize<InputVariable>(json);

        // Assert
        Assert.Throws<NotSupportedException>(() => result?.Default);
    }

    [Fact]
    public void ItShouldNotBePossibleToDeserializeDefaultFromArray()
    {
        // Arrange
        var json = "{\"default\": [1, 2, 4]}";

        // Act
        var result = JsonSerializer.Deserialize<InputVariable>(json);

        // Assert
        Assert.Throws<NotSupportedException>(() => result?.Default);
    }

    private class MyCustomType
    {
        public string Name { get; set; } = string.Empty;
    }
}
