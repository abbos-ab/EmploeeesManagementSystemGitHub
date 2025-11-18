namespace EmployeesManagementSystem.Models;

public class Department
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    public ICollection<UserDepartmentRole> UserDepartmentRoles { get; set; } = new List<UserDepartmentRole>();
}