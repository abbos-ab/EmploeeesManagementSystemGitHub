using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Models;

namespace EmployeesManagementSystem.Services.Interfaces;

public interface IDocumentService
{
    Task<List<DocumentResponse>> GetAll();
    Task<List<FileResponse>> GetBySenderId();
    Task<List<FileResponse>> GetByReceiverId();
    Task<bool> UpLoad(DocumentRequest request);
    Task<bool> Send(Guid id, Guid receiverUserId);
    Task<Document> Download(Guid id);
    Task<Document> StampFile(Guid fileId);
    string GetContentType(string fileName);
}