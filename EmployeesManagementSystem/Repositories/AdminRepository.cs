using EmployeesManagementSystem.Contexts;
using EmployeesManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeesManagementSystem.Repositories
{
    public class AdminRepository
    {
        private readonly AppDbContext _context;
        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAll()
        {
            return _context.Users.ToList();
        }

        public async Task<List<Files>> GetAllFiles()
        {
            return _context.File.ToList();
        }

        public async Task<User> Add(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task Delete(Guid id)
        {
            await _context.Users
                .Where(c => c.Id == id)
                .ExecuteDeleteAsync();
        }
    }
}
