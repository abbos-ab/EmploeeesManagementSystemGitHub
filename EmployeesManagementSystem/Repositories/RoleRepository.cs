using AutoMapper;
using EmployeesManagementSystem.Contexts;
using EmployeesManagementSystem.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EmployeesManagementSystem.Repositories;

public class RoleRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public RoleRepository(AppDbContext context, IMapper mapper)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<List<RoleDto>> GetAssignableRoles()
    {
        var roles = await _context.Roles
            .Where(r => r.Name != "SuperAdmin")
            .ToListAsync();

        var result = _mapper.Map<List<RoleDto>>(roles);
        return result;
    }
}