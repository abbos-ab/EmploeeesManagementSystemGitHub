using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeesManagementSystem.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class UserController : BaseController
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    [HttpGet]
    public Task<List<UserResponse>> GetAll()
    {
        return _service.GetAll();
    }

    [HttpGet]
    public Task<UserResponse> GetById(Guid id)
    {
        return _service.GetById(id);
    }

    [HttpPost]
    public async Task<IActionResult> Registration([FromBody] CreateUserRequest createUser)
    {
        var createdUser = await _service.Create(createUser);
        return Ok(createdUser);
    }

    [HttpPut]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest updateUser)
    {
        var updatedUser = await _service.Update(id, updateUser);
        return Ok(updatedUser);
    }

    [HttpDelete]
    [Authorize(Roles = "SuperAdmin")] // Only SuperAdmin can delete users
    public Task Delete(Guid id)
    {
        return _service.Delete(id);
    }
}