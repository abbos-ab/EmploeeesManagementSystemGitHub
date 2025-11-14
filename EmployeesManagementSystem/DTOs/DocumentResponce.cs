namespace EmployeesManagementSystem.DTOs
{
    public class DocumentResponce
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Content { get; set; }
        public byte[] Data { get; set; }

    }
}
