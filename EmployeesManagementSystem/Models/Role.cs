namespace EmployeesManagementSystem.Models
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<UserDeportmentRole> UserDeportmentRoles { get; set; } = new List<UserDeportmentRole>();
    }
}
