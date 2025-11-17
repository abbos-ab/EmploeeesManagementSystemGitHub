using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeesManagementSystem.Controllers;

public class RoleController : BaseController
{
    private readonly RoleService _service;

    public RoleController(RoleService service)
    {
        _service = service;
    }

    [HttpGet]
    public Task<List<RoleDto>> GetAll()
    {
        return _service.GetAll();
    }
}