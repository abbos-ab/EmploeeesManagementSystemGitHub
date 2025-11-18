using Microsoft.AspNetCore.Mvc;
using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace EmployeesManagementSystem.Controllers;

public class AuthController(ILoginService service) : BaseController
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<string>> Login(UserLogin request)
    {
        var token = await service.LoginAsync(request);
        if (token is null)
            return BadRequest("User not found");
        return Ok(token);
    }
}