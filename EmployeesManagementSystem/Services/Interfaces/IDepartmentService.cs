using EmployeesManagementSystem.DTOs;

namespace EmployeesManagementSystem.Services.Interfaces;

public interface IDepartmentService
{
    Task<List<DepartmentResponse>> GetAll();
    Task<DepartmentResponse> GetById(Guid id);
    Task<DepartmentResponse> Create(string name);
    Task<DepartmentResponse> Update(Guid id, string name);
    Task Delete(Guid id);
}