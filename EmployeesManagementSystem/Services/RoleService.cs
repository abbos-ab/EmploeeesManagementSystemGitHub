using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Repositories;

namespace EmployeesManagementSystem.Services;

public class RoleService
{
    private readonly RoleRepository _repository;

    public RoleService(RoleRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<RoleDto>> GetAll()
    {
        var results = await _repository.GetAssignableRoles();
        return results;
    }
}