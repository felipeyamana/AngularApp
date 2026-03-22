using Application.Common.Dispatchers.Implementations;
using Application.Common.Interfaces;
using FluentAssertions;
using Moq;

namespace Application.UnitTests.Common.Dispatchers;

public class CommandDispatcherTests
{
    public sealed record TestCommand(string Value);

    public sealed class TestResult
    {
        public string Value { get; init; } = string.Empty;
    }

    [Fact]
    public async Task Dispatch_ShouldResolveHandlerAndInvokeHandle()
    {
        // Arrange
        var command = new TestCommand("value");
        var expected = new TestResult { Value = "handled-" + command.Value };

        var handlerMock = new Mock<ICommandHandler<TestCommand, TestResult>>();
        handlerMock
            .Setup(h => h.Handle(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(ICommandHandler<TestCommand, TestResult>)))
            .Returns(handlerMock.Object);

        var dispatcher = new CommandDispatcher(serviceProviderMock.Object);

        // Act
        var result = await dispatcher.Dispatch<TestCommand, TestResult>(command, CancellationToken.None);

        // Assert
        result.Should().BeSameAs(expected);
        handlerMock.Verify(h => h.Handle(command, It.IsAny<CancellationToken>()), Times.Once);
    }
}

