namespace EmployeesManagementSystem.Models;

public class UserDepartmentRole
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid RoleId { get; set; }

    public User User { get; set; }
    public Department Department { get; set; }
    public Role Role { get; set; }
}