using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Repositories.Interfaces;
using EmployeesManagementSystem.Services;
using FluentAssertions;
using NSubstitute;

namespace EmployeesManagementSystem.Tests.Services;

public class RoleServiceTests
{
    private readonly IRoleRepository _repository;
    private readonly RoleService _sut;

    public RoleServiceTests()
    {
        _repository = Substitute.For<IRoleRepository>();
        _sut = new RoleService(_repository);
    }

    [Fact]
    public async Task GetAll_ShouldReturnListOfAssignableRoles()
    {
        // Arrange
        var roles = new List<RoleResponse>
        {
            new() { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Admin" },
            new() { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "User" }
        };

        _repository.GetAssignableRoles().Returns(roles);

        // Act
        var result = await _sut.GetAll();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(roles);
        result.Should().NotContain(r => r.Name == "SuperAdmin");
        await _repository.Received(1).GetAssignableRoles();
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoRolesExist()
    {
        // Arrange
        _repository.GetAssignableRoles().Returns(new List<RoleResponse>());

        // Act
        var result = await _sut.GetAll();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        await _repository.Received(1).GetAssignableRoles();
    }

    [Fact]
    public async Task GetAll_ShouldCallRepositoryOnce()
    {
        // Arrange
        var roles = new List<RoleResponse>
        {
            new() { Id = Guid.NewGuid(), Name = "Admin" }
        };
        _repository.GetAssignableRoles().Returns(roles);

        // Act
        await _sut.GetAll();
        await _sut.GetAll();

        // Assert
        await _repository.Received(2).GetAssignableRoles();
    }

    [Fact]
    public async Task GetAll_ShouldReturnOnlyAdminAndUserRoles()
    {
        // Arrange
        var roles = new List<RoleResponse>
        {
            new() { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Admin" },
            new() { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "User" }
        };
        _repository.GetAssignableRoles().Returns(roles);

        // Act
        var result = await _sut.GetAll();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(r => r.Name == "Admin");
        result.Should().Contain(r => r.Name == "User");
        result.Should().NotContain(r => r.Id == Guid.Parse("11111111-1111-1111-1111-111111111111"));
    }

    [Fact]
    public async Task GetAll_ShouldReturnRolesWithCorrectIds()
    {
        // Arrange
        var adminId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var userId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var roles = new List<RoleResponse>
        {
            new() { Id = adminId, Name = "Admin" },
            new() { Id = userId, Name = "User" }
        };
        _repository.GetAssignableRoles().Returns(roles);

        // Act
        var result = await _sut.GetAll();

        // Assert
        result.Should().Contain(r => r.Id == adminId && r.Name == "Admin");
        result.Should().Contain(r => r.Id == userId && r.Name == "User");
    }
}