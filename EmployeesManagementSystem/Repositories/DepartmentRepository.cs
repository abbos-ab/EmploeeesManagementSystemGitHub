using Microsoft.EntityFrameworkCore;
using EmployeesManagementSystem.Contexts;
using EmployeesManagementSystem.Models;
namespace EmployeesManagementSystem.Repositories
{
    public class DepartmentRepository
    {
        private readonly AppDbContext _context;
        public DepartmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Departament>> GetAll()
        {
            return await _context.Departaments.ToListAsync();
        }

        public async Task<Departament> GetById(Guid id)
        {
            return await _context.Departaments
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Departament> Add(Departament department)
        {
            _context.Departaments.Add(department);
            await _context.SaveChangesAsync();
            return department;
        }


        public async Task<Departament> Update(Departament department)
        {
            _context.Departaments.Update(department);
            await _context.SaveChangesAsync();
            return department;
        }

        public async Task Delete(Departament department)
        {
            _context.Departaments.Remove(department);
            await _context.SaveChangesAsync();
        }

    }
}