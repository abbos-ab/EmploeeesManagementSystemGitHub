namespace EmployeesManagementSystem.Models;

public class OperationList
{
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public string ActionType { get; set; }
    public DateTime DateTime { get; set; }
    public Document File { get; set; }
    public User Sender { get; set; }
    public User Receiver { get; set; }
}