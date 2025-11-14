using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeesManagementSystem.Controllers
{
    public class AssignmentsController : BaseController
    {
        public readonly AssignmentsService _service;
        public readonly CurrentUserService _currentUserService;

        public AssignmentsController(AssignmentsService departmentService, CurrentUserService currentUserService)
        {
            _service = departmentService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<List<AssignmentsResponce>> GetAll()
        {
            var res = await _service.GetAll();
            return res;
        }

        [HttpPost]
        public async Task<IActionResult> Assign([FromForm]AssignmentsRequest createDepartment)
        {
            var createdDepartment = await _service.Create(createDepartment);
            return Ok(createdDepartment);
        }

        [HttpDelete]
        public async Task<IActionResult> UnAssign(Guid id)
        {
            await _service.Delete(id);
            return Ok();
        }
    }
}
