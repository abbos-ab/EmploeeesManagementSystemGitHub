namespace EmployeesManagementSystem.DTOs;

public class OperationListResponse
{
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public string ActionType { get; set; }
    public DateTime DateTime { get; set; }
}