namespace EmployeesManagementSystem.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Password { get; set; }

        public Role Role { get; set; } = null!;
        public Guid RoleId {  get; set; }

        public ICollection<Files> Files { get; set; } = new List<Files>();

        public ICollection<OperationList> operationLists { get; set; } = new List<OperationList>();
    }
}
