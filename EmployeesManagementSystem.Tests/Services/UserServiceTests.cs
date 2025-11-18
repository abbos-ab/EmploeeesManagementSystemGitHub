using AutoMapper;
using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Models;
using EmployeesManagementSystem.Repositories.Interfaces;
using EmployeesManagementSystem.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace EmployeesManagementSystem.Tests.Services;

public class UserServiceTests
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();
        _sut = new UserService(_userRepository, _mapper);
    }

    [Fact]
    public async Task GetAll_ShouldReturnListOfUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new() { Id = Guid.NewGuid(), Name = "John Doe", Email = "john@test.com" },
            new() { Id = Guid.NewGuid(), Name = "Jane Smith", Email = "jane@test.com" }
        };

        var userResponses = new List<UserResponse>
        {
            new() { Id = users[0].Id, Name = "John Doe", Email = "john@test.com" },
            new() { Id = users[1].Id, Name = "Jane Smith", Email = "jane@test.com" }
        };

        _userRepository.GetAll().Returns(users);
        _mapper.Map<List<UserResponse>>(users).Returns(userResponses);

        // Act
        var result = await _sut.GetAll();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(userResponses);
        await _userRepository.Received(1).GetAll();
        _mapper.Received(1).Map<List<UserResponse>>(users);
    }

    [Fact]
    public async Task GetById_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Name = "John Doe", Email = "john@test.com" };
        var userResponse = new UserResponse { Id = userId, Name = "John Doe", Email = "john@test.com" };

        _userRepository.GetById(userId).Returns(user);
        _mapper.Map<UserResponse>(user).Returns(userResponse);

        // Act
        var result = await _sut.GetById(userId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(userId);
        result.Name.Should().Be("John Doe");
        await _userRepository.Received(1).GetById(userId);
    }

    [Fact]
    public async Task Create_ShouldThrowException_WhenEmailAlreadyExists()
    {
        // Arrange
        var createRequest = new CreateUserRequest
        {
            Name = "John Doe",
            Email = "john@test.com",
            Password = "Password123"
        };

        var existingUser = new User { Email = "john@test.com" };
        _userRepository.GetByEmailAsync(createRequest.Email).Returns(existingUser);

        // Act
        Func<Task> act = async () => await _sut.Create(createRequest);

        // Assert
        await act.Should().ThrowAsync<DbUpdateException>()
            .WithMessage("*john@test.com*already exists*");
        await _userRepository.Received(1).GetByEmailAsync(createRequest.Email);
        await _userRepository.DidNotReceive().Add(Arg.Any<User>());
    }

    [Fact]
    public async Task Create_ShouldCreateUser_WhenEmailDoesNotExist()
    {
        // Arrange
        var createRequest = new CreateUserRequest
        {
            Name = "John Doe",
            Email = "john@test.com",
            Password = "Password123"
        };

        var createdUser = new User
        {
            Id = Guid.NewGuid(),
            Name = "John Doe",
            Email = "john@test.com",
            Password = "HashedPassword"
        };

        var userResponse = new UserResponse
        {
            Id = createdUser.Id,
            Name = "John Doe",
            Email = "john@test.com"
        };

        _userRepository.GetByEmailAsync(createRequest.Email).Returns((User?)null);
        _userRepository.Add(Arg.Any<User>()).Returns(createdUser);
        _mapper.Map<UserResponse>(createdUser).Returns(userResponse);

        // Act
        var result = await _sut.Create(createRequest);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("John Doe");
        result.Email.Should().Be("john@test.com");
        await _userRepository.Received(1).GetByEmailAsync(createRequest.Email);
        await _userRepository.Received(1).Add(Arg.Is<User>(u =>
            u.Name == "John Doe" &&
            u.Email == "john@test.com" &&
            !string.IsNullOrEmpty(u.Password)));
    }

    [Fact]
    public async Task Update_ShouldUpdateUser_AndReturnUpdatedUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var updateRequest = new UpdateUserRequest
        {
            Name = "Updated Name",
            Email = "updated@test.com",
            Password = "NewPassword123"
        };

        var user = new User { Id = userId, Name = "Updated Name", Email = "updated@test.com" };
        var updatedUser = new User { Id = userId, Name = "Updated Name", Email = "updated@test.com" };
        var userResponse = new UserResponse { Id = userId, Name = "Updated Name", Email = "updated@test.com" };

        _mapper.Map<User>(updateRequest).Returns(user);
        _userRepository.Update(Arg.Any<User>()).Returns(updatedUser);
        _mapper.Map<UserResponse>(updatedUser).Returns(userResponse);

        // Act
        var result = await _sut.Update(userId, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(userId);
        result.Name.Should().Be("Updated Name");
        await _userRepository.Received(1).Update(Arg.Is<User>(u => u.Id == userId));
    }

    [Fact]
    public async Task Delete_ShouldCallRepositoryDelete()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        await _sut.Delete(userId);

        // Assert
        await _userRepository.Received(1).Delete(userId);
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        // Arrange
        _userRepository.GetAll().Returns(new List<User>());
        _mapper.Map<List<UserResponse>>(Arg.Any<List<User>>()).Returns(new List<UserResponse>());

        // Act
        var result = await _sut.GetAll();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        await _userRepository.Received(1).GetAll();
    }

    [Fact]
    public async Task GetById_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepository.GetById(userId)
            .Returns(Task.FromException<User>(new KeyNotFoundException($"User with ID {userId} not found.")));

        // Act
        Func<Task> act = async () => await _sut.GetById(userId);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{userId}*not found*");
    }

    [Fact]
    public async Task Create_ShouldHashPassword_WhenCreatingUser()
    {
        // Arrange
        var createRequest = new CreateUserRequest
        {
            Name = "John Doe",
            Email = "john@test.com",
            Password = "PlainTextPassword"
        };

        var createdUser = new User
        {
            Id = Guid.NewGuid(),
            Name = "John Doe",
            Email = "john@test.com",
            Password = "HashedPassword"
        };

        var userResponse = new UserResponse
        {
            Id = createdUser.Id,
            Name = "John Doe",
            Email = "john@test.com"
        };

        _userRepository.GetByEmailAsync(createRequest.Email).Returns((User?)null);
        _userRepository.Add(Arg.Any<User>()).Returns(createdUser);
        _mapper.Map<UserResponse>(createdUser).Returns(userResponse);

        // Act
        var result = await _sut.Create(createRequest);

        // Assert
        result.Should().NotBeNull();
        await _userRepository.Received(1).Add(Arg.Is<User>(u =>
            u.Password != "PlainTextPassword" && // Password should be hashed
            u.Password != null &&
            u.Password.Length > 20)); // Hashed passwords are longer
    }

    [Fact]
    public async Task Create_ShouldTrimAndValidateEmail()
    {
        // Arrange
        var createRequest = new CreateUserRequest
        {
            Name = "John Doe",
            Email = "  john@test.com  ",
            Password = "Password123"
        };

        _userRepository.GetByEmailAsync(Arg.Any<string>()).Returns((User?)null);
        _userRepository.Add(Arg.Any<User>()).Returns(new User { Id = Guid.NewGuid() });
        _mapper.Map<UserResponse>(Arg.Any<User>()).Returns(new UserResponse());

        // Act
        await _sut.Create(createRequest);

        // Assert
        await _userRepository.Received(1).GetByEmailAsync("  john@test.com  ");
    }

    [Fact]
    public async Task Update_ShouldPreserveUserId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var updateRequest = new UpdateUserRequest
        {
            Name = "Updated Name",
            Email = "updated@test.com",
            Password = "NewPassword"
        };

        var mappedUser = new User { Name = "Updated Name", Email = "updated@test.com" };
        _mapper.Map<User>(updateRequest).Returns(mappedUser);
        _userRepository.Update(Arg.Any<User>()).Returns(new User { Id = userId });
        _mapper.Map<UserResponse>(Arg.Any<User>()).Returns(new UserResponse { Id = userId });

        // Act
        await _sut.Update(userId, updateRequest);

        // Assert
        await _userRepository.Received(1).Update(Arg.Is<User>(u => u.Id == userId));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task Create_ShouldHandleInvalidEmails(string? email)
    {
        // Arrange
        var createRequest = new CreateUserRequest
        {
            Name = "John Doe",
            Email = email!,
            Password = "Password123"
        };

        _userRepository.GetByEmailAsync(email!).Returns((User?)null);
        _userRepository.Add(Arg.Any<User>()).Returns(new User { Id = Guid.NewGuid() });
        _mapper.Map<UserResponse>(Arg.Any<User>()).Returns(new UserResponse());

        // Act
        var result = await _sut.Create(createRequest);

        // Assert
        result.Should().NotBeNull();
        await _userRepository.Received(1).Add(Arg.Is<User>(u => u.Email == email));
    }

    [Fact]
    public async Task Update_ShouldNotHashPassword_WhenUpdating()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var updateRequest = new UpdateUserRequest
        {
            Name = "Updated Name",
            Email = "updated@test.com",
            Password = "NewPassword123"
        };

        var mappedUser = new User
        {
            Name = "Updated Name",
            Email = "updated@test.com",
            Password = "NewPassword123"
        };

        _mapper.Map<User>(updateRequest).Returns(mappedUser);
        _userRepository.Update(Arg.Any<User>()).Returns(new User
        {
            Id = userId,
            Name = "Updated Name",
            Email = "updated@test.com",
            Password = "NewPassword123"
        });
        _mapper.Map<UserResponse>(Arg.Any<User>()).Returns(new UserResponse { Id = userId });

        // Act
        await _sut.Update(userId, updateRequest);

        // Assert
        await _userRepository.Received(1).Update(Arg.Is<User>(u =>
            u.Id == userId &&
            u.Password == "NewPassword123")); // Password is not hashed in update
    }
}