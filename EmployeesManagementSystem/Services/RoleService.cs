using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Repositories.Interfaces;
using EmployeesManagementSystem.Services.Interfaces;

namespace EmployeesManagementSystem.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _repository;

    public RoleService(IRoleRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<RoleResponse>> GetAll()
    {
        var results = await _repository.GetAssignableRoles();
        return results;
    }
}