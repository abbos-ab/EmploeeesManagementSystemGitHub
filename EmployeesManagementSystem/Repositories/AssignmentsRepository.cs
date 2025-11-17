using EmployeesManagementSystem.Models;
using EmployeesManagementSystem.Contexts;
using EmployeesManagementSystem.DTOs;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace EmployeesManagementSystem.Repositories;

public class AssignmentsRepository
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;

    public AssignmentsRepository(AppDbContext dbContext, IMapper mapper)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }

    public async Task<List<UserDepartmentRole>> GetAll()
    {
        var result = await _dbContext.UserDepartmentRoles.ToListAsync();
        return result;
    }

    public async Task<UserDepartmentRole> GetForCheck(Guid userId, Guid departmentId, Guid roleId)
    {
        var result = await _dbContext.UserDepartmentRoles
            .FirstOrDefaultAsync(udr => udr.UserId == userId
                                        && udr.DepartmentId == departmentId
                                        && udr.RoleId == roleId);
        return result;
    }

    public async Task<AssignmentsResponse> Add(AssignmentsRequest request)
    {
        var entity = _mapper.Map<UserDepartmentRole>(request);
        _dbContext.UserDepartmentRoles.Add(entity);
        await _dbContext.SaveChangesAsync();
        var response = _mapper.Map<AssignmentsResponse>(entity);
        return response;
    }

    public async Task Delete(Guid id)
    {
        await _dbContext.UserDepartmentRoles
            .Where(c => c.Id == id)
            .ExecuteDeleteAsync();
    }

    public async Task<List<UserDepartmentRole>> GetByUserIdAsync(Guid userId)
    {
        return await _dbContext.UserDepartmentRoles
            .Include(udr => udr.Department)
            .Include(udr => udr.Role)
            .Include(udr => udr.User)
            .Where(udr => udr.UserId == userId)
            .ToListAsync();
    }
}