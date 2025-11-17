using EmployeesManagementSystem.Models;

namespace EmployeesManagementSystem.Repositories.Interfaces;

public interface IOperationRepository
{
    Task SaveOperation(OperationList operation);
    Task<Guid> GetSenderId(Guid fileId);
    Task<List<Guid>> GetFileIdReceivers(Guid receiverId);
    Task<Guid> GetIdReceiver(Guid fileId);
    Task<List<Guid>> GetFileIdsBySender(Guid senderId);
}