using AutoMapper;
using EmployeesManagementSystem.Contexts;
using EmployeesManagementSystem.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EmployeesManagementSystem.Repositories
{
    public class RoleRepository
    {
        private readonly AppDbContext _context;
        private IMapper _mapper;
        public RoleRepository(AppDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<List<RoleDTO>> GetAsignableRoles()
        {
            var roles = await _context.Roles
                .Where(r => r.Name != "SuperAdmin")
                .ToListAsync();

            var result = _mapper.Map<List<RoleDTO>>(roles);
            return result;

        }
    }
}
