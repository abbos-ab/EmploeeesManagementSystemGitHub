namespace EmployeesManagementSystem.DTOs
{
    public class OperationListResponce
    {
        public Guid Id { get; set; }
        public Guid FileID { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string AcrionType { get; set; }
        public DateTime DateTime { get; set; }
    }
}
