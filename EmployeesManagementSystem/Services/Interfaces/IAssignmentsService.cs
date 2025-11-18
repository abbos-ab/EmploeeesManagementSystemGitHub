using EmployeesManagementSystem.DTOs;

namespace EmployeesManagementSystem.Services.Interfaces;

public interface IAssignmentsService
{
    Task<List<AssignmentsResponse>> GetAll();
    Task<AssignmentsResponse> Create(AssignmentsRequest request);
    Task Delete(Guid id);
    Task<List<AssignmentsResponse>> GetUserAssignmentsAsync(Guid userId);
}