using AutoMapper;
using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Models;
using EmployeesManagementSystem.Repositories.Interfaces;
using EmployeesManagementSystem.Services;
using FluentAssertions;
using NSubstitute;

namespace EmployeesManagementSystem.Tests.Services;

public class DepartmentServiceTests
{
    private readonly IDepartmentRepository _repository;
    private readonly IMapper _mapper;
    private readonly DepartmentService _sut;

    public DepartmentServiceTests()
    {
        _repository = Substitute.For<IDepartmentRepository>();
        _mapper = Substitute.For<IMapper>();
        _sut = new DepartmentService(_repository, _mapper);
    }

    [Fact]
    public async Task GetAll_ShouldReturnListOfDepartments()
    {
        // Arrange
        var departments = new List<Department>
        {
            new() { Id = Guid.NewGuid(), Name = "IT" },
            new() { Id = Guid.NewGuid(), Name = "HR" }
        };

        var departmentResponses = new List<DepartmentResponse>
        {
            new() { Id = departments[0].Id, Name = "IT" },
            new() { Id = departments[1].Id, Name = "HR" }
        };

        _repository.GetAll().Returns(departments);
        _mapper.Map<List<DepartmentResponse>>(departments).Returns(departmentResponses);

        // Act
        var result = await _sut.GetAll();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(departmentResponses);
        await _repository.Received(1).GetAll();
    }

    [Fact]
    public async Task GetById_ShouldReturnDepartment_WhenDepartmentExists()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        var department = new Department { Id = departmentId, Name = "IT" };
        var departmentResponse = new DepartmentResponse { Id = departmentId, Name = "IT" };

        _repository.GetById(departmentId).Returns(department);
        _mapper.Map<DepartmentResponse>(department).Returns(departmentResponse);

        // Act
        var result = await _sut.GetById(departmentId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(departmentId);
        result.Name.Should().Be("IT");
        await _repository.Received(1).GetById(departmentId);
    }

    [Fact]
    public async Task Create_ShouldCreateDepartment_AndReturnResponse()
    {
        // Arrange
        var departmentName = "IT";
        var createdDepartment = new Department { Id = Guid.NewGuid(), Name = departmentName };
        var departmentResponse = new DepartmentResponse { Id = createdDepartment.Id, Name = departmentName };

        _repository.Add(Arg.Any<Department>()).Returns(createdDepartment);
        _mapper.Map<DepartmentResponse>(createdDepartment).Returns(departmentResponse);

        // Act
        var result = await _sut.Create(departmentName);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(departmentName);
        await _repository.Received(1).Add(Arg.Is<Department>(d => d.Name == departmentName));
    }

    [Fact]
    public async Task Update_ShouldUpdateDepartment_AndReturnResponse()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        var departmentName = "Updated IT";
        var existingDepartment = new Department { Id = departmentId, Name = "IT" };
        var updatedDepartment = new Department { Id = departmentId, Name = departmentName };
        var departmentResponse = new DepartmentResponse { Id = departmentId, Name = departmentName };

        _repository.GetById(departmentId).Returns(existingDepartment);
        _repository.Update(Arg.Any<Department>()).Returns(updatedDepartment);
        _mapper.Map<DepartmentResponse>(updatedDepartment).Returns(departmentResponse);

        // Act
        var result = await _sut.Update(departmentId, departmentName);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(departmentName);
        await _repository.Received(1).GetById(departmentId);
        await _repository.Received(1).Update(Arg.Is<Department>(d => d.Name == departmentName));
    }

    [Fact]
    public async Task Delete_ShouldDeleteDepartment()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        var department = new Department { Id = departmentId, Name = "IT" };

        _repository.GetById(departmentId).Returns(department);

        // Act
        await _sut.Delete(departmentId);

        // Assert
        await _repository.Received(1).GetById(departmentId);
        await _repository.Received(1).Delete(department);
    }

    [Fact]
    public async Task Delete_ShouldThrowException_WhenDepartmentNotFound()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        _repository.GetById(departmentId)
            .Returns(Task.FromException<Department>(new KeyNotFoundException()));

        // Act
        Func<Task> act = async () => await _sut.Delete(departmentId);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoDepartmentsExist()
    {
        // Arrange
        _repository.GetAll().Returns(new List<Department>());
        _mapper.Map<List<DepartmentResponse>>(Arg.Any<List<Department>>())
            .Returns(new List<DepartmentResponse>());

        // Act
        var result = await _sut.GetAll();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetById_ShouldThrowException_WhenDepartmentNotFound()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        _repository.GetById(departmentId)
            .Returns(Task.FromException<Department>(new KeyNotFoundException()));

        // Act
        Func<Task> act = async () => await _sut.GetById(departmentId);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Theory]
    [InlineData("IT Department")]
    [InlineData("Human Resources")]
    [InlineData("Sales & Marketing")]
    public async Task Create_ShouldCreateDepartmentWithVariousNames(string departmentName)
    {
        // Arrange
        var createdDepartment = new Department { Id = Guid.NewGuid(), Name = departmentName };
        var departmentResponse = new DepartmentResponse { Id = createdDepartment.Id, Name = departmentName };

        _repository.Add(Arg.Any<Department>()).Returns(createdDepartment);
        _mapper.Map<DepartmentResponse>(createdDepartment).Returns(departmentResponse);

        // Act
        var result = await _sut.Create(departmentName);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(departmentName);
    }

    [Fact]
    public async Task Update_ShouldThrowException_WhenDepartmentNotFound()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        _repository.GetById(departmentId)
            .Returns(Task.FromException<Department>(new KeyNotFoundException()));

        // Act
        Func<Task> act = async () => await _sut.Update(departmentId, "Updated Name");

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Create_ShouldGenerateNewGuid_ForEachDepartment()
    {
        // Arrange
        var firstDepartment = new Department { Id = Guid.NewGuid(), Name = "Dept1" };
        var secondDepartment = new Department { Id = Guid.NewGuid(), Name = "Dept2" };

        _repository.Add(Arg.Any<Department>())
            .Returns(firstDepartment, secondDepartment);
        _mapper.Map<DepartmentResponse>(Arg.Any<Department>())
            .Returns(
                new DepartmentResponse { Id = firstDepartment.Id, Name = "Dept1" },
                new DepartmentResponse { Id = secondDepartment.Id, Name = "Dept2" });

        // Act
        var result1 = await _sut.Create("Dept1");
        var result2 = await _sut.Create("Dept2");

        // Assert
        result1.Id.Should().NotBe(result2.Id);
    }
}