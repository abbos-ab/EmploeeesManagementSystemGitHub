using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Models;

namespace EmployeesManagementSystem.Repositories.Interfaces;

public interface IAssignmentsRepository
{
    Task<List<UserDepartmentRole>> GetAll();
    Task<UserDepartmentRole> GetForCheck(Guid userId, Guid departmentId, Guid roleId);
    Task<AssignmentsResponse> Add(AssignmentsRequest request);
    Task Delete(Guid id);
    Task<List<UserDepartmentRole>> GetByUserIdAsync(Guid userId);
}