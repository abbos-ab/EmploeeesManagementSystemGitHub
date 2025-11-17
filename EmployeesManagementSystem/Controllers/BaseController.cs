using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeesManagementSystem.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
public abstract class BaseController : ControllerBase;