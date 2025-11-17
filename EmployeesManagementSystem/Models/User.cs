namespace EmployeesManagementSystem.Models;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Email { get; set; }
    public string? Password { get; set; }

    public ICollection<UserDepartmentRole> UserDepartmentRoles { get; set; } = new List<UserDepartmentRole>();
    public ICollection<OperationList> SendOperations { get; set; } = new List<OperationList>();
    public ICollection<OperationList> ReceiveOperations { get; set; } = new List<OperationList>();
}