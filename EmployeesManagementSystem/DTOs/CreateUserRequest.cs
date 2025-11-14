namespace EmployeesManagementSystem.DTOs
{
    public class CreateUserRequest
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public Guid RoleId { get; set; }
        public Guid DepartamentId { get; set; }
    }
}
