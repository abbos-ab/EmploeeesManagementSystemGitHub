using EmployeesManagementSystem.Models;

namespace EmployeesManagementSystem.Repositories.Interfaces;

public interface IDepartmentRepository
{
    Task<List<Department>> GetAll();
    Task<Department> GetById(Guid id);
    Task<Department> Add(Department department);
    Task<Department> Update(Department department);
    Task Delete(Department department);
}