using EmployeesManagementSystem.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using System.Security.Claims;

namespace EmployeesManagementSystem.Tests.Services;

public class CurrentUserServiceTests
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly CurrentUserService _sut;

    public CurrentUserServiceTests()
    {
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        _sut = new CurrentUserService(_httpContextAccessor);
    }

    [Fact]
    public void GetUserIdFromToken_ShouldReturnUserId_WhenClaimExists()
    {
        // Arrange
        var expectedUserId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, expectedUserId.ToString())
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContext = Substitute.For<HttpContext>();
        httpContext.User.Returns(claimsPrincipal);
        _httpContextAccessor.HttpContext.Returns(httpContext);

        // Act
        var result = _sut.GetUserIdFromToken();

        // Assert
        result.Should().Be(expectedUserId);
    }

    [Fact]
    public void GetUserIdFromToken_ShouldThrowException_WhenClaimDoesNotExist()
    {
        // Arrange
        var claims = new List<Claim>();
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContext = Substitute.For<HttpContext>();
        httpContext.User.Returns(claimsPrincipal);
        _httpContextAccessor.HttpContext.Returns(httpContext);

        // Act
        Action act = () => _sut.GetUserIdFromToken();

        // Assert
        act.Should().Throw<UnauthorizedAccessException>()
            .WithMessage("*User ID claim not found*");
    }

    [Fact]
    public void GetUserIdFromToken_ShouldThrowException_WhenHttpContextIsNull()
    {
        // Arrange
        _httpContextAccessor.HttpContext.Returns((HttpContext)null);

        // Act
        Action act = () => _sut.GetUserIdFromToken();

        // Assert
        act.Should().Throw<UnauthorizedAccessException>();
    }

    [Fact]
    public void GetUserIdFromToken_ShouldThrowException_WhenUserIsNull()
    {
        // Arrange
        var httpContext = Substitute.For<HttpContext>();
        httpContext.User.Returns((ClaimsPrincipal)null);
        _httpContextAccessor.HttpContext.Returns(httpContext);

        // Act
        Action act = () => _sut.GetUserIdFromToken();

        // Assert
        act.Should().Throw<UnauthorizedAccessException>();
    }

    [Fact]
    public void GetUserIdFromToken_ShouldThrowException_WhenClaimValueIsNotValidGuid()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "invalid-guid")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContext = Substitute.For<HttpContext>();
        httpContext.User.Returns(claimsPrincipal);
        _httpContextAccessor.HttpContext.Returns(httpContext);

        // Act
        Action act = () => _sut.GetUserIdFromToken();

        // Assert
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void GetUserIdFromToken_ShouldReturnCorrectGuid_WithUppercaseGuid()
    {
        // Arrange
        var expectedUserId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, expectedUserId.ToString().ToUpper())
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContext = Substitute.For<HttpContext>();
        httpContext.User.Returns(claimsPrincipal);
        _httpContextAccessor.HttpContext.Returns(httpContext);

        // Act
        var result = _sut.GetUserIdFromToken();

        // Assert
        result.Should().Be(expectedUserId);
    }

    [Fact]
    public void GetUserIdFromToken_ShouldWorkWithMultipleClaims()
    {
        // Arrange
        var expectedUserId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "John Doe"),
            new(ClaimTypes.Email, "john@test.com"),
            new(ClaimTypes.NameIdentifier, expectedUserId.ToString()),
            new(ClaimTypes.Role, "Admin")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContext = Substitute.For<HttpContext>();
        httpContext.User.Returns(claimsPrincipal);
        _httpContextAccessor.HttpContext.Returns(httpContext);

        // Act
        var result = _sut.GetUserIdFromToken();

        // Assert
        result.Should().Be(expectedUserId);
    }
}