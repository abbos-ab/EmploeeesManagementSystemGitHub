using EmployeesManagementSystem.DTOs;

namespace EmployeesManagementSystem.Services.Interfaces;

public interface ILoginService
{
    Task<string> LoginAsync(UserLogin request);
}