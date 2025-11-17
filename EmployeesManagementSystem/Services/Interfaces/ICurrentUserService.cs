namespace EmployeesManagementSystem.Services.Interfaces;

public interface ICurrentUserService
{
    Guid GetUserIdFromToken();
}