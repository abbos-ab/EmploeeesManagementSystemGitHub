using Microsoft.EntityFrameworkCore;
using EmployeesManagementSystem.Contexts;
using EmployeesManagementSystem.Models;
using EmployeesManagementSystem.Repositories.Interfaces;

namespace EmployeesManagementSystem.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly AppDbContext _context;

    public DepartmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Department>> GetAll()
    {
        return await _context.Departments.ToListAsync();
    }

    public async Task<Department> GetById(Guid id)
    {
        var department = await _context.Departments
            .FirstOrDefaultAsync(c => c.Id == id);
        
        if (department == null)
            throw new KeyNotFoundException($"Department with ID {id} not found.");
        
        return department;
    }

    public async Task<Department> Add(Department department)
    {
        _context.Departments.Add(department);
        await _context.SaveChangesAsync();
        return department;
    }


    public async Task<Department> Update(Department department)
    {
        _context.Departments.Update(department);
        await _context.SaveChangesAsync();
        return department;
    }

    public async Task Delete(Department department)
    {
        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();
    }
}