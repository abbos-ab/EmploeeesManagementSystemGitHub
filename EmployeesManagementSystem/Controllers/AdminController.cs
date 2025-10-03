using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace EmployeesManagementSystem.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;

        public AdminController(AdminService userService)
        {
            _adminService = userService;
        }

        [HttpGet("Get-All-Users")]
        public Task<List<AdminResponce>> GetAll()
        {
            return _adminService.GetAlL();
        }

        [HttpPost("Create-Users")]
        public async Task<IActionResult> Create(CreateAdminRequest createUser)
        {
            var createdUser = await _adminService.Create(createUser);
            return Ok(createdUser);
        }

        [HttpDelete("Delete Users")]
        public Task Delete([FromQuery] Guid id)
        {
            return _adminService.Delete(id);
        }

        [HttpGet]
        public Task<List<FileResponce>> GetAllFiles()
        {
            return _adminService.GetAllFiles();
        }
    }
}
