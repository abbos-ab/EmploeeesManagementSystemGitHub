using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeesManagementSystem.Controllers
{
    public class UserController : BaseController
    {
        private readonly UserService _service;

        public UserController(UserService service)
        {
            _service = service;
        }

        [HttpGet]
        public Task<List<UserResponce>> GetAll()
        {
            return _service.GetAlL();
        }

        [HttpGet]
        public Task<UserResponce> GetById(Guid id)
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
        public async Task<IActionResult> Update(Guid id, [FromBody] UserRequest upDateUser)
        {
            var updatedUser = await _service.Update(id, upDateUser);
            return Ok(updatedUser);
        }

        [HttpDelete]
        public Task Delete(Guid id)
        {
            return _service.Delete(id);
        }
    }
}
