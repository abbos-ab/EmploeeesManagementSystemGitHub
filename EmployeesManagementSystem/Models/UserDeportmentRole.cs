namespace EmployeesManagementSystem.Models
{
    public class UserDeportmentRole
    {
        public Guid Id { get; set; }
        public Guid IdUser { get; set; }
        public Guid IdDeportment { get; set; }
        public Guid IdRole { get; set; }



        public User User { get; set; }
        public Departament Departament { get; set; }
        public Role Role { get; set; }
        ICollection<User> Users { get; set; } = new List<User>();
    }
}