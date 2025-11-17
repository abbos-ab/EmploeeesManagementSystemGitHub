using EmployeesManagementSystem.Models;

namespace EmployeesManagementSystem.Repositories.Interfaces;

public interface IDocumentRepository
{
    Task<List<Document>> GetAll();
    Task<List<Document>> GetByFileIds(List<Guid> fileIds);
    Task<List<Document>> GetByReceiverId(List<Guid> fileId);
    Task SaveFile(Document file);
    Task InsertFile(Document doc);
    Task<Document> Download(Guid id, Guid receiverId);
    Task<Document> DownloadById(Guid id, Guid receiverId);
}