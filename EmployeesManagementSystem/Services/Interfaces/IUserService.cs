using EmployeesManagementSystem.DTOs;

namespace EmployeesManagementSystem.Services.Interfaces;

public interface IUserService
{
    Task<List<UserResponse>> GetAll();
    Task<UserResponse> GetById(Guid id);
    Task<UserResponse> Create(CreateUserRequest createUser);
    Task<UserResponse> Update(Guid id, UpdateUserRequest updateUser);
    Task Delete(Guid id);
}