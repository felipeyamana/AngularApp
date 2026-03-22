using Application.Logs.Services;
using Domain.Entities.Logs;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.UnitTests.Logs.Services;

public class LogServiceTests
{
    private readonly DbContextOptions<ApplicationDbContext> _options;
    private readonly ApplicationDbContext _dbContext;
    private readonly Mock<ILogger<LogService>> _loggerMock;
    private readonly LogService _service;

    public LogServiceTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ApplicationDbContext(_options);
        _loggerMock = new Mock<ILogger<LogService>>();
        _service = new LogService(_dbContext, _loggerMock.Object);
    }

    [Fact]
    public async Task LogAsync_ShouldPersistLog()
    {
        // Arrange
        var log = new Log
        {
            LogTypeId = (int)LogTypes.System,
            Action = "Test",
            Description = "Test log"
        };

        // Act
        await _service.LogAsync(log, CancellationToken.None);

        // Assert
        _dbContext.Logs.Should().ContainSingle(l => l.Action == "Test" && l.Description == "Test log");
    }

    [Fact]
    public async Task LogAsync_WhenSaveChangesThrows_ShouldLogError()
    {
        // Arrange
        var failingContextMock = new Mock<ApplicationDbContext>(_options) { CallBase = true };
        failingContextMock
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("boom"));

        var service = new LogService(failingContextMock.Object, _loggerMock.Object);

        var log = new Log
        {
            LogTypeId = (int)LogTypes.System,
            Action = "Test failure",
            Description = "Should trigger logging"
        };

        // Act
        await service.LogAsync(log, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((_, _) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task LogSuccessAsync_ShouldCreateAndPersistSuccessLog()
    {
        // Act
        await _service.LogSuccessAsync(
            LogTypes.User,
            "Action",
            "Message",
            userId: "user",
            ip: "1.2.3.4",
            cancellationToken: CancellationToken.None);

        // Assert
        var log = _dbContext.Logs.Single();
        log.LogTypeId.Should().Be((int)LogTypes.User);
        log.Action.Should().Be("Action");
        log.Description.Should().Be("Message");
        log.UserId.Should().Be("user");
        log.IpAddress.Should().Be("1.2.3.4");
        log.ActionSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task LogFailureAsync_ShouldCreateAndPersistFailureLog()
    {
        // Act
        await _service.LogFailureAsync(
            LogTypes.User,
            "Action",
            "Error message",
            userId: "user",
            ip: "5.6.7.8",
            cancellationToken: CancellationToken.None);

        // Assert
        var log = _dbContext.Logs.Single();
        log.LogTypeId.Should().Be((int)LogTypes.User);
        log.Action.Should().Be("Action");
        log.Description.Should().Be("Error message");
        log.UserId.Should().Be("user");
        log.IpAddress.Should().Be("5.6.7.8");
        log.ActionSuccess.Should().BeFalse();
    }
}

