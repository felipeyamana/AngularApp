using Domain.Entities.Logs;
using Domain.Enums;
using Domain.Factories;
using FluentAssertions;

namespace Domain.UnitTests;

public class LogFactoryTests
{
    [Fact]
    public void CreateSuccess_ShouldPopulateExpectedFields()
    {
        // Arrange
        var type = LogTypes.User;
        var action = "Some action";
        var message = "Some message";
        var userId = "user-123";
        var ip = "1.2.3.4";

        // Act
        Log result = LogFactory.CreateSuccess(type, action, message, userId, ip);

        // Assert
        result.LogTypeId.Should().Be((int)type);
        result.Action.Should().Be(action);
        result.Description.Should().Be(message);
        result.UserId.Should().Be(userId);
        result.IpAddress.Should().Be(ip);
        result.ActionSuccess.Should().BeTrue();
        result.EventDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void CreateFailure_ShouldPopulateExpectedFields()
    {
        // Arrange
        var type = LogTypes.System;
        var action = "Failure action";
        var error = "Something went wrong";
        var userId = "user-456";
        var ip = "5.6.7.8";

        // Act
        Log result = LogFactory.CreateFailure(type, action, error, userId, ip);

        // Assert
        result.LogTypeId.Should().Be((int)type);
        result.Action.Should().Be(action);
        result.Description.Should().Be(error);
        result.UserId.Should().Be(userId);
        result.IpAddress.Should().Be(ip);
        result.ActionSuccess.Should().BeFalse();
        result.EventDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}
