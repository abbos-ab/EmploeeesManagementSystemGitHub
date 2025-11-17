using EmployeesManagementSystem.Contexts;
using EmployeesManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeesManagementSystem.Repositories;

public class UserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public Task<List<User>> GetAll()
    {
        return _context.Users.ToListAsync();
    }

    public async Task<User> GetById(Guid id)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(c => c.Id == id);
        
        if (user == null)
            throw new KeyNotFoundException($"User with ID {id} not found.");
        
        return user;
    }

    public async Task<User> Add(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> Update(User user)
    {
        _context.Users.Update(user);
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