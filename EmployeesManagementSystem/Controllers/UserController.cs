using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using MySql.Data.MySqlClient;
using System.Security.Claims;
using Microsoft.AspNetCore.StaticFiles;
using EmployeesManagementSystem.Contexts;
using EmployeesManagementSystem.Models;
namespace EmployeesManagementSystem.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserController : ControllerBase
    {
        
        public readonly string _connectionString = "server=localhost; database=userms; user=root; password=;";
        public readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }
        [HttpPost("Send-file")]
        public async Task<IActionResult> SendFile([FromForm] SendFileRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (request.formFile == null || request.formFile.Length == 0)
                return BadRequest("Fayl yuborilmadi");

            var success = await _userService.SendFileAsynce(request);
            if (!success)
                return NotFound("User topilmadi yoki fayl saqlanmadi");

            return Ok("Fayl muvoffaqqiyatli yuborildi");
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> DownloadFile(Guid id)
        {
            var doc =await _userService.DownloadAsync(id);
            if (doc == null)
                return NotFound();
            return File(doc.Data, doc.Content, doc.Name);
        }
    }
}
