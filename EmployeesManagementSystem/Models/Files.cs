using Microsoft.AspNetCore.Http.HttpResults;
using System.Data;

namespace EmployeesManagementSystem.Models
{
    public class Files
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Content { get; set; }
        public Guid CreatedBy { get; set; }
        public User User { get; set; } = null!;
        
        public User Receiver { get; set; }
        public Guid ReceiverId { get; set; }
        public byte[] Data { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<OperationList> Operations { get; set; } = new List<OperationList>();
    }
}
