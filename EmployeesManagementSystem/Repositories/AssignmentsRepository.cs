using EmployeesManagementSystem.Models;
using EmployeesManagementSystem.Contexts;
using EmployeesManagementSystem.DTOs;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace EmployeesManagementSystem.Repositories
{
    public class AssignmentsRepository
    {
        public readonly AppDbContext _DbContext;
        public readonly IMapper _mapper;
        public AssignmentsRepository(AppDbContext dbContext, IMapper mapper)
        {
            _mapper = mapper;
            _DbContext = dbContext;
        }

        public async Task<List<UserDeportmentRole>> GetAll()
        {
            var res = await _DbContext.UserDeportmentRoles.ToListAsync();
            return res;
        }

        public async Task<UserDeportmentRole?> GetForCheck(Guid userId, Guid departmentId, Guid roleId)
        {
            var res = await _DbContext.UserDeportmentRoles
                .FirstOrDefaultAsync(udr => udr.IdUser == userId
                    && udr.IdDeportment == departmentId
                    && udr.IdRole == roleId);
            return res;
        }

        public async Task<AssignmentsResponce> Add(AssignmentsRequest createDepartment)
        {
            var entity = _mapper.Map<UserDeportmentRole>(createDepartment);
            _DbContext.UserDeportmentRoles.Add(entity);
            await _DbContext.SaveChangesAsync();
            var responce = _mapper.Map<AssignmentsResponce>(entity);
            return responce;
        }

        public async Task Delete(Guid id)
        {
            await _DbContext.UserDeportmentRoles
                .Where(c => c.Id == id)
                .ExecuteDeleteAsync();
        }
        public async Task<List<UserDeportmentRole?>> GetByUserIdAsync(Guid userId)
        {
            return await _DbContext.UserDeportmentRoles
               .Include(udr => udr.Departament)
               .Include(udr => udr.Role)
               .Include(udr => udr.User)
               .Where(udr => udr.IdUser == userId)
               .ToListAsync();

        }
    }
}
