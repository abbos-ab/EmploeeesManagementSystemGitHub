namespace EmployeesManagementSystem.Models
{
    public class OperationList
    {
        public Guid id { get; set; }
        public DateTime DateTime { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid FileID { get; set; }
        public Files File { get; set; } = null!;
        public string ActionType { get; set; }
    }
}
