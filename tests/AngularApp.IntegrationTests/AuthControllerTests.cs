using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Application.Common.Results;
using Application.Users.Commands.RegisterUser;
using Application.Users.Queries.UserLogin;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace AngularApp.IntegrationTests;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_WhenSuccess_ShouldReturnOkWithMessageAndUserId()
    {
        // Arrange
        var mockFactory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Replace ICommandDispatcher with a mock
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(Application.Common.Dispatchers.Interfaces.ICommandDispatcher));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                var dispatcherMock = new Mock<Application.Common.Dispatchers.Interfaces.ICommandDispatcher>();

                dispatcherMock
                    .Setup(d => d.Dispatch<RegisterUserRequest, Result<string>>(
                        It.IsAny<RegisterUserRequest>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result<string>.SuccessResult("new-user-id"));

                services.AddSingleton(dispatcherMock.Object);
            });
        });

        var client = mockFactory.CreateClient();

        var request = new RegisterUserRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "Test",
            LastName = "User"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("message").GetString().Should().Be("User registered successfully!");
        body.GetProperty("userId").GetString().Should().Be("new-user-id");
    }

    [Fact]
    public async Task Register_WhenFailure_ShouldReturnBadRequestWithErrors()
    {
        // Arrange
        var mockFactory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(Application.Common.Dispatchers.Interfaces.ICommandDispatcher));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                var dispatcherMock = new Mock<Application.Common.Dispatchers.Interfaces.ICommandDispatcher>();

                dispatcherMock
                    .Setup(d => d.Dispatch<RegisterUserRequest, Result<string>>(
                        It.IsAny<RegisterUserRequest>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result<string>.Failure("error1", "error2"));

                services.AddSingleton(dispatcherMock.Object);
            });
        });

        var client = mockFactory.CreateClient();

        var request = new RegisterUserRequest
        {
            Email = "bad@example.com",
            Password = "Password123!",
            FirstName = "Bad",
            LastName = "User"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        var errors = body.GetProperty("errors").EnumerateArray().Select(e => e.GetString()).ToList();
        errors.Should().Contain(new[] { "error1", "error2" });
    }

    [Fact]
    public async Task Login_WhenSuccess_ShouldReturnOkAndSetAuthCookie()
    {
        // Arrange
        var mockFactory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(Application.Common.Dispatchers.Interfaces.IQueryDispatcher));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                var dispatcherMock = new Mock<Application.Common.Dispatchers.Interfaces.IQueryDispatcher>();

                dispatcherMock
                    .Setup(d => d.Dispatch<UserLoginRequest, Result<string>>(
                        It.IsAny<UserLoginRequest>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result<string>.SuccessResult("fake-jwt-token"));

                services.AddSingleton(dispatcherMock.Object);
            });
        });

        var client = mockFactory.CreateClient(new() { AllowAutoRedirect = false });

        var request = new UserLoginRequest
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("token").GetString().Should().Be("fake-jwt-token");

        // Check cookie was set
        response.Headers.TryGetValues("Set-Cookie", out var setCookieHeaders).Should().BeTrue();
        setCookieHeaders!.Any(h => h.StartsWith("auth_token=")).Should().BeTrue();
    }

    [Fact]
    public async Task Login_WhenFailure_ShouldReturnUnauthorizedWithErrors()
    {
        // Arrange
        var mockFactory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(Application.Common.Dispatchers.Interfaces.IQueryDispatcher));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                var dispatcherMock = new Mock<Application.Common.Dispatchers.Interfaces.IQueryDispatcher>();

                dispatcherMock
                    .Setup(d => d.Dispatch<UserLoginRequest, Result<string>>(
                        It.IsAny<UserLoginRequest>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result<string>.Failure("invalid credentials"));

                services.AddSingleton(dispatcherMock.Object);
            });
        });

        var client = mockFactory.CreateClient(new() { AllowAutoRedirect = false });

        var request = new UserLoginRequest
        {
            Email = "test@example.com",
            Password = "wrong"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        var errors = body.GetProperty("errors").EnumerateArray().Select(e => e.GetString()).ToList();
        errors.Should().Contain("invalid credentials");
    }
}

