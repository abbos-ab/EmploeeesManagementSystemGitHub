using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeesManagementSystem.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class AssignmentsController : BaseController
{
    private readonly IAssignmentsService _service;

    public AssignmentsController(IAssignmentsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<List<AssignmentsResponse>> GetAll()
    {
        var result = await _service.GetAll();
        return result;
    }

    [HttpPost]
    public async Task<IActionResult> Assign([FromForm] AssignmentsRequest request)
    {
        var createdAssignment = await _service.Create(request);
        return Ok(createdAssignment);
    }

    [HttpDelete]
    public async Task<IActionResult> UnAssign(Guid id)
    {
        await _service.Delete(id);
        return Ok();
    }
}