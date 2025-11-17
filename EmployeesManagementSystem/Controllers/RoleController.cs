using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeesManagementSystem.Controllers;

public class RoleController : BaseController
{
    private readonly IRoleService _service;

    public RoleController(IRoleService service)
    {
        _service = service;
    }

    [HttpGet]
    public Task<List<RoleResponse>> GetAll()
    {
        return _service.GetAll();
    }
}