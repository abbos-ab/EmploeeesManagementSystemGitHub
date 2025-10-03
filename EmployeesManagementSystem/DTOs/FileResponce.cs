using EmployeesManagementSystem.Models;

namespace EmployeesManagementSystem.DTOs
{
    public class FileResponce
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Content { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid ReceiverId { get; set; }

    }
}
