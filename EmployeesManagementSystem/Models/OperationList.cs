namespace EmployeesManagementSystem.Models
{
    public class OperationList
    {
        public Guid Id { get; set; }
        public Guid FileID { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string AcrionType { get; set; } = null!;
        public DateTime DateTime { get; set; }

        public Document File { get; set; } = null!;
        public User UserSend { get; set; } = null!;
        public User UserReceive { get; set; } = null!;
    }
}
