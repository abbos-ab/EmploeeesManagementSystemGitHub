using AutoMapper;
using EmployeesManagementSystem.Contexts;
using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Models;
using EmployeesManagementSystem.Repositories.Interfaces;
using EmployeesManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EmployeesManagementSystem.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;

    public UserService(IUserRepository repository, IMapper mapper, AppDbContext context)
    {
        _repository = repository;
        _mapper = mapper;
        _context = context;
    }

    public async Task<List<UserResponse>> GetAll()
    {
        var user = await _repository.GetAll();
        var response = _mapper.Map<List<UserResponse>>(user);
        return response;
    }

    public async Task<UserResponse> GetById(Guid id)
    {
        var user = await _repository.GetById(id);
        var response = _mapper.Map<UserResponse>(user);
        return response;
    }

    public async Task<UserResponse> Create(CreateUserRequest createUser)
    {
        var existEmail = await _repository.GetByEmailAsync(createUser.Email);
        if (existEmail is not null)
        {
            throw new DbUpdateException($"User with email {createUser.Email} already exists.");
        }

        // Validate that role and department exist
        var roleExists = await _context.Roles.AnyAsync(r => r.Id == createUser.RoleId);
        var departmentExists = await _context.Departments.AnyAsync(d => d.Id == createUser.DepartmentId);

        if (!roleExists)
            throw new KeyNotFoundException($"Role with ID {createUser.RoleId} not found.");
        if (!departmentExists)
            throw new KeyNotFoundException($"Department with ID {createUser.DepartmentId} not found.");

        var user = new User();
        var hashedPassword = new PasswordHasher<User>()
            .HashPassword(user, createUser.Password);
        user.Name = createUser.Name;
        user.Email = createUser.Email;
        user.Password = hashedPassword;

        var createdUser = await _repository.Add(user);

        // Create UserDepartmentRole entry
        var userDepartmentRole = new UserDepartmentRole
        {
            UserId = createdUser.Id,
            DepartmentId = createUser.DepartmentId,
            RoleId = createUser.RoleId
        };
        _context.UserDepartmentRoles.Add(userDepartmentRole);
        await _context.SaveChangesAsync();

        var response = _mapper.Map<UserResponse>(createdUser);
        return response;
    }

    public async Task<UserResponse> Update(Guid id, UpdateUserRequest updateUser)
    {
        // Get existing user
        var existingUser = await _repository.GetById(id);

        // Update fields
        existingUser.Name = updateUser.Name;
        existingUser.Email = updateUser.Email;

        // Only update password if provided (and hash it)
        if (!string.IsNullOrWhiteSpace(updateUser.Password))
        {
            var hashedPassword = new PasswordHasher<User>()
                .HashPassword(existingUser, updateUser.Password);
            existingUser.Password = hashedPassword;
        }

        var updatedUser = await _repository.Update(existingUser);
        var response = _mapper.Map<UserResponse>(updatedUser);
        return response;
    }

    public async Task Delete(Guid id)
    {
        await _repository.Delete(id);
    }
}