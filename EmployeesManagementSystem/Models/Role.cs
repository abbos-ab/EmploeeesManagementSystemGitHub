namespace EmployeesManagementSystem.Models;

public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<UserDepartmentRole> UserDepartmentRoles { get; set; } = new List<UserDepartmentRole>();
}