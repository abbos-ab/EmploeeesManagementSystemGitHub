using EmployeesManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeesManagementSystem.Controllers;

public class DepartmentController : BaseController
{
    private readonly DepartmentService _service;

    public DepartmentController(DepartmentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var departments = await _service.GetAll();
        return Ok(departments);
    }

    [HttpGet]
    public async Task<IActionResult> GetById([FromQuery] Guid id)
    {
        var department = await _service.GetById(id);
        return Ok(department);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] string name)
    {
        var createdDepartment = await _service.Create(name);
        return Ok(createdDepartment);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromForm] Guid id, [FromForm] string name)
    {
        var updatedDepartment = await _service.Update(id, name);
        return Ok(updatedDepartment);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] Guid id)
    {
        await _service.Delete(id);
        return Ok("Department deleted successfully");
    }
}