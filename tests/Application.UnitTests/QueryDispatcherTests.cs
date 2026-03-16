using Application.Common.Dispatchers.Implementations;
using Application.Common.Interfaces;
using FluentAssertions;
using Moq;

namespace Application.UnitTests.Common.Dispatchers;

public class QueryDispatcherTests
{
    public sealed record TestQuery(string Input);

    public sealed class TestResult
    {
        public string Value { get; init; } = string.Empty;
    }

    [Fact]
    public async Task Dispatch_ShouldResolveHandlerAndInvokeHandle()
    {
        // Arrange
        var query = new TestQuery("input");
        var expected = new TestResult { Value = "handled-" + query.Input };

        var handlerMock = new Mock<IQueryHandler<TestQuery, TestResult>>();
        handlerMock
            .Setup(h => h.Handle(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IQueryHandler<TestQuery, TestResult>)))
            .Returns(handlerMock.Object);

        var dispatcher = new QueryDispatcher(serviceProviderMock.Object);

        // Act
        var result = await dispatcher.Dispatch<TestQuery, TestResult>(query, CancellationToken.None);

        // Assert
        result.Should().BeSameAs(expected);
        handlerMock.Verify(h => h.Handle(query, It.IsAny<CancellationToken>()), Times.Once);
    }
}

