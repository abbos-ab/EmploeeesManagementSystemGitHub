using EmployeesManagementSystem.DTOs;

namespace EmployeesManagementSystem.Services.Interfaces;

public interface IRoleService
{
    Task<List<RoleResponse>> GetAll();
}