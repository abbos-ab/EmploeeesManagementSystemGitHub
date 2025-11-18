namespace EmployeesManagementSystem.DTOs;

public class AssignmentsRequest
{
    public Guid UserId { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid RoleId { get; set; }
}