using AutoMapper;
using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Models;
using EmployeesManagementSystem.Repositories.Interfaces;
using EmployeesManagementSystem.Services;
using FluentAssertions;
using NSubstitute;
using Org.BouncyCastle.Security;

namespace EmployeesManagementSystem.Tests.Services;

public class AssignmentsServiceTests
{
    private readonly IAssignmentsRepository _repository;
    private readonly IMapper _mapper;
    private readonly AssignmentsService _sut;

    public AssignmentsServiceTests()
    {
        _repository = Substitute.For<IAssignmentsRepository>();
        _mapper = Substitute.For<IMapper>();
        _sut = new AssignmentsService(_repository, _mapper);
    }

    [Fact]
    public async Task GetAll_ShouldReturnListOfAssignments()
    {
        // Arrange
        var assignments = new List<UserDepartmentRole>
        {
            new()
            {
                Id = Guid.NewGuid(), UserId = Guid.NewGuid(), DepartmentId = Guid.NewGuid(), RoleId = Guid.NewGuid()
            },
            new()
            {
                Id = Guid.NewGuid(), UserId = Guid.NewGuid(), DepartmentId = Guid.NewGuid(), RoleId = Guid.NewGuid()
            }
        };

        var assignmentResponses = new List<AssignmentsResponse>
        {
            new()
            {
                Id = assignments[0].Id, UserId = assignments[0].UserId, DepartmentId = assignments[0].DepartmentId,
                RoleId = assignments[0].RoleId
            },
            new()
            {
                Id = assignments[1].Id, UserId = assignments[1].UserId, DepartmentId = assignments[1].DepartmentId,
                RoleId = assignments[1].RoleId
            }
        };

        _repository.GetAll().Returns(assignments);
        _mapper.Map<List<AssignmentsResponse>>(assignments).Returns(assignmentResponses);

        // Act
        var result = await _sut.GetAll();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(assignmentResponses);
        await _repository.Received(1).GetAll();
    }

    [Fact]
    public async Task Create_ShouldThrowException_WhenAssignmentAlreadyExists()
    {
        // Arrange
        var request = new AssignmentsRequest
        {
            UserId = Guid.NewGuid(),
            DepartmentId = Guid.NewGuid(),
            RoleId = Guid.Parse("22222222-2222-2222-2222-222222222222")
        };

        var existingAssignment = new UserDepartmentRole
        {
            UserId = request.UserId,
            DepartmentId = request.DepartmentId,
            RoleId = request.RoleId
        };

        _repository.GetForCheck(request.UserId, request.DepartmentId, request.RoleId)
            .Returns(existingAssignment);

        // Act
        Func<Task> act = async () => await _sut.Create(request);

        // Assert
        await act.Should().ThrowAsync<InvalidParameterException>()
            .WithMessage("*already exists*");
        await _repository.Received(1).GetForCheck(request.UserId, request.DepartmentId, request.RoleId);
        await _repository.DidNotReceive().Add(Arg.Any<AssignmentsRequest>());
    }

    [Fact]
    public async Task Create_ShouldThrowException_WhenTryingToAssignSuperAdminRole()
    {
        // Arrange
        var superAdminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var request = new AssignmentsRequest
        {
            UserId = Guid.NewGuid(),
            DepartmentId = Guid.NewGuid(),
            RoleId = superAdminRoleId
        };

        _repository.GetForCheck(request.UserId, request.DepartmentId, request.RoleId)
            .Returns((UserDepartmentRole?)null);

        // Act
        Func<Task> act = async () => await _sut.Create(request);

        // Assert
        await act.Should().ThrowAsync<InvalidParameterException>()
            .WithMessage("*SuperAdmin*");
        await _repository.DidNotReceive().Add(Arg.Any<AssignmentsRequest>());
    }

    [Fact]
    public async Task Create_ShouldCreateAssignment_WhenValidRequest()
    {
        // Arrange
        var request = new AssignmentsRequest
        {
            UserId = Guid.NewGuid(),
            DepartmentId = Guid.NewGuid(),
            RoleId = Guid.Parse("22222222-2222-2222-2222-222222222222")
        };

        var createdAssignment = new AssignmentsResponse
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            DepartmentId = request.DepartmentId,
            RoleId = request.RoleId
        };

        _repository.GetForCheck(request.UserId, request.DepartmentId, request.RoleId)
            .Returns((UserDepartmentRole?)null);
        _repository.Add(request).Returns(createdAssignment);
        _mapper.Map<AssignmentsResponse>(createdAssignment).Returns(createdAssignment);

        // Act
        var result = await _sut.Create(request);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(request.UserId);
        result.DepartmentId.Should().Be(request.DepartmentId);
        result.RoleId.Should().Be(request.RoleId);
        await _repository.Received(1).Add(request);
    }

    [Fact]
    public async Task Delete_ShouldCallRepositoryDelete()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();

        // Act
        await _sut.Delete(assignmentId);

        // Assert
        await _repository.Received(1).Delete(assignmentId);
    }

    [Fact]
    public async Task GetUserAssignmentsAsync_ShouldReturnUserAssignments()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var assignments = new List<UserDepartmentRole>
        {
            new() { Id = Guid.NewGuid(), UserId = userId, DepartmentId = Guid.NewGuid(), RoleId = Guid.NewGuid() }
        };

        var assignmentResponses = new List<AssignmentsResponse>
        {
            new()
            {
                Id = assignments[0].Id, UserId = userId, DepartmentId = assignments[0].DepartmentId,
                RoleId = assignments[0].RoleId
            }
        };

        _repository.GetByUserIdAsync(userId).Returns(assignments);
        _mapper.Map<List<AssignmentsResponse>>(assignments).Returns(assignmentResponses);

        // Act
        var result = await _sut.GetUserAssignmentsAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.All(r => r.UserId == userId).Should().BeTrue();
        await _repository.Received(1).GetByUserIdAsync(userId);
    }

    [Fact]
    public async Task GetUserAssignmentsAsync_ShouldReturnEmptyList_WhenUserHasNoAssignments()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _repository.GetByUserIdAsync(userId).Returns(new List<UserDepartmentRole>());
        _mapper.Map<List<AssignmentsResponse>>(Arg.Any<List<UserDepartmentRole>>())
            .Returns(new List<AssignmentsResponse>());

        // Act
        var result = await _sut.GetUserAssignmentsAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoAssignmentsExist()
    {
        // Arrange
        _repository.GetAll().Returns(new List<UserDepartmentRole>());
        _mapper.Map<List<AssignmentsResponse>>(Arg.Any<List<UserDepartmentRole>>())
            .Returns(new List<AssignmentsResponse>());

        // Act
        var result = await _sut.GetAll();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Theory]
    [InlineData("22222222-2222-2222-2222-222222222222")] // Admin
    [InlineData("33333333-3333-3333-3333-333333333333")] // User
    public async Task Create_ShouldSucceed_WithValidNonSuperAdminRoles(string roleIdString)
    {
        // Arrange
        var roleId = Guid.Parse(roleIdString);
        var request = new AssignmentsRequest
        {
            UserId = Guid.NewGuid(),
            DepartmentId = Guid.NewGuid(),
            RoleId = roleId
        };

        var createdAssignment = new AssignmentsResponse
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            DepartmentId = request.DepartmentId,
            RoleId = roleId
        };

        _repository.GetForCheck(request.UserId, request.DepartmentId, request.RoleId)
            .Returns((UserDepartmentRole?)null);
        _repository.Add(request).Returns(createdAssignment);
        _mapper.Map<AssignmentsResponse>(createdAssignment).Returns(createdAssignment);

        // Act
        var result = await _sut.Create(request);

        // Assert
        result.Should().NotBeNull();
        result.RoleId.Should().Be(roleId);
    }

    [Fact]
    public async Task Create_ShouldVerifyNoExistingAssignment_BeforeCreating()
    {
        // Arrange
        var request = new AssignmentsRequest
        {
            UserId = Guid.NewGuid(),
            DepartmentId = Guid.NewGuid(),
            RoleId = Guid.Parse("22222222-2222-2222-2222-222222222222")
        };

        _repository.GetForCheck(request.UserId, request.DepartmentId, request.RoleId)
            .Returns((UserDepartmentRole?)null);
        _repository.Add(request).Returns(new AssignmentsResponse { Id = Guid.NewGuid() });

        // Act
        await _sut.Create(request);

        // Assert
        await _repository.Received(1).GetForCheck(request.UserId, request.DepartmentId, request.RoleId);
        await _repository.Received(1).Add(request);
    }

    [Fact]
    public async Task Delete_ShouldNotThrow_WhenDeletingNonExistentAssignment()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();

        // Act
        Func<Task> act = async () => await _sut.Delete(assignmentId);

        // Assert
        await act.Should().NotThrowAsync();
        await _repository.Received(1).Delete(assignmentId);
    }

    [Fact]
    public async Task GetUserAssignmentsAsync_ShouldReturnMultipleAssignments_WhenUserHasMany()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var assignments = new List<UserDepartmentRole>
        {
            new() { Id = Guid.NewGuid(), UserId = userId, DepartmentId = Guid.NewGuid(), RoleId = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), UserId = userId, DepartmentId = Guid.NewGuid(), RoleId = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), UserId = userId, DepartmentId = Guid.NewGuid(), RoleId = Guid.NewGuid() }
        };

        var assignmentResponses = assignments.Select(a => new AssignmentsResponse
        {
            Id = a.Id,
            UserId = a.UserId,
            DepartmentId = a.DepartmentId,
            RoleId = a.RoleId
        }).ToList();

        _repository.GetByUserIdAsync(userId).Returns(assignments);
        _mapper.Map<List<AssignmentsResponse>>(assignments).Returns(assignmentResponses);

        // Act
        var result = await _sut.GetUserAssignmentsAsync(userId);

        // Assert
        result.Should().HaveCount(3);
        result.All(r => r.UserId == userId).Should().BeTrue();
    }
}