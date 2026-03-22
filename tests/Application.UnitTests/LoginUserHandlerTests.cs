using Application.Common.Results;
using Application.Users.Queries.UserLogin;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Application.UnitTests.Users.Queries.UserLogin;

public class LoginUserHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly LoginUserHandler _handler;

    public LoginUserHandlerTests()
    {
        _userManagerMock = CreateUserManagerMock();
        _signInManagerMock = CreateSignInManagerMock(_userManagerMock.Object);
        _configurationMock = new Mock<IConfiguration>();

        // Basic JWT configuration so GenerateJwtToken can run
        _configurationMock.Setup(c => c["Jwt:Key"]).Returns("a-very-secret-key-1234567890123456");
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("test-audience");
        _configurationMock.Setup(c => c["Jwt:ExpireMinutes"]).Returns("60");

        _handler = new LoginUserHandler(
            _signInManagerMock.Object,
            _userManagerMock.Object,
            _configurationMock.Object);
    }

    [Fact]
    public async Task Handle_UserNotFound_ShouldReturnFailure()
    {
        // Arrange
        var request = new UserLoginRequest
        {
            Email = "missing@example.com",
            Password = "password"
        };

        _userManagerMock
            .Setup(m => m.FindByEmailAsync(request.Email))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        Result<string> result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.Should().Be("Invalid login attempt.");
    }

    [Fact]
    public async Task Handle_InvalidPassword_ShouldReturnFailure()
    {
        // Arrange
        var request = new UserLoginRequest
        {
            Email = "user@example.com",
            Password = "wrong-password"
        };

        var user = new ApplicationUser { Id = "user-id", Email = request.Email };

        _userManagerMock
            .Setup(m => m.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);

        _signInManagerMock
            .Setup(m => m.CheckPasswordSignInAsync(user, request.Password, false))
            .ReturnsAsync(SignInResult.Failed);

        // Act
        Result<string> result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.Should().Be("Invalid login attempt.");
    }

    [Fact]
    public async Task Handle_ValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var request = new UserLoginRequest
        {
            Email = "user@example.com",
            Password = "correct-password"
        };

        var user = new ApplicationUser { Id = "user-id", Email = request.Email };

        _userManagerMock
            .Setup(m => m.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);

        _signInManagerMock
            .Setup(m => m.CheckPasswordSignInAsync(user, request.Password, false))
            .ReturnsAsync(SignInResult.Success);

        _userManagerMock
            .Setup(m => m.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "Admin" });

        // Act
        Result<string> result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNullOrWhiteSpace();
    }

    private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        return new Mock<UserManager<ApplicationUser>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }

    private static Mock<SignInManager<ApplicationUser>> CreateSignInManagerMock(UserManager<ApplicationUser> userManager)
    {
        var contextAccessor = new Mock<IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();

        return new Mock<SignInManager<ApplicationUser>>(
            userManager,
            contextAccessor.Object,
            claimsFactory.Object,
            null!, null!, null!, null!);
    }
}

