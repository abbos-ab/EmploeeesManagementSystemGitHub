using AutoMapper;
using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Models;
using EmployeesManagementSystem.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EmployeesManagementSystem.Services;

public class UserService
{
    private readonly UserRepository _repository;
    private readonly IMapper _mapper;

    public UserService(UserRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
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

        var user = new User();
        var hashedPassword = new PasswordHasher<User>()
            .HashPassword(user, createUser.Password);
        user.Name = createUser.Name;
        user.Email = createUser.Email;
        user.Password = hashedPassword;
        var createdUser = await _repository.Add(user);
        var response = _mapper.Map<UserResponse>(createdUser);
        return response;
    }

    public async Task<UserResponse> Update(Guid id, UserRequest updateUser)
    {
        var user = _mapper.Map<User>(updateUser);
        user.Id = id;
        var updatedUser = await _repository.Update(user);
        var response = _mapper.Map<UserResponse>(updatedUser);
        return response;
    }

    public async Task Delete(Guid id)
    {
        await _repository.Delete(id);
    }
}