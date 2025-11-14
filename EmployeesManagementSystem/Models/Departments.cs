namespace EmployeesManagementSystem.Models
{
    public class Departament
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }

        public ICollection<UserDeportmentRole> UserDeportmentRoles { get; set; } = new List<UserDeportmentRole>();
    }
}
