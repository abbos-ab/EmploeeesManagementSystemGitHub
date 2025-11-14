using Microsoft.AspNetCore.Mvc;
using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;

namespace EmployeesManagementSystem.Controllers
{
    public class AuthController(LoginService _service) : BaseController
    {
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<string>> Login(Userlog request)
        {
            var token = await _service.LoginAsync(request);
            if (token is null)
                return BadRequest("User not found");
            return Ok(token);
        }

    }
}
