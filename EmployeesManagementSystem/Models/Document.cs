namespace EmployeesManagementSystem.Models;

public class Document
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Content { get; set; }
    public byte[] Data { get; set; }

    public ICollection<OperationList> Operations { get; set; } = new List<OperationList>();
}