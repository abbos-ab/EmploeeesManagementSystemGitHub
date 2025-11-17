using EmployeesManagementSystem.Models;

namespace EmployeesManagementSystem.Repositories.Interfaces;

public interface IUserRepository
{
    Task Delete(Guid id);
    Task<User> Update(User user);
    Task<User> Add(User user);
    Task<User> GetById(Guid id);
    Task<List<User>> GetAll();
    Task<User> GetByEmailAsync(string email);
}