namespace EmployeesManagementSystem.DTOs
{
    public class SendFileRequest
    {
       public IFormFile formFile { get; set; }
        public Guid ReceiverId { get; set; }


    }
}
