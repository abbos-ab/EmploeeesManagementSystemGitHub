namespace EmployeesManagementSystem.DTOs
{
    public class CreateAdminRequest
    {
        public string Name { get; set; }
        public string PassWord { get; set; }
        public Guid RoleId { get; set; }
    }
}
