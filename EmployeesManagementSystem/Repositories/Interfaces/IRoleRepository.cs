using EmployeesManagementSystem.DTOs;

namespace EmployeesManagementSystem.Repositories.Interfaces;

public interface IRoleRepository
{
    Task<List<RoleResponse>> GetAssignableRoles();
}