using Microsoft.EntityFrameworkCore;
using EmployeesManagementSystem.Contexts;
using EmployeesManagementSystem.Models;

namespace EmployeesManagementSystem.Repositories;

public class OperationRepository
{
    private readonly AppDbContext _context;

    public OperationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task SaveOperation(OperationList operation)
    {
        _context.Operations.Add(operation);
        await _context.SaveChangesAsync();
    }

    public async Task<Guid> GetSenderId(Guid fileId)
    {
        var operation = await _context.Operations
            .FirstOrDefaultAsync(op => op.FileId == fileId);
        if (operation == null)
        {
            throw new KeyNotFoundException("Operation not found.");
        }

        return operation.SenderId;
    }

    public async Task<List<Guid>> GetFileIdReceivers(Guid receiverId)
    {
        return await _context.Operations
            .Where(op => op.ReceiverId == receiverId)
            .Select(op => op.FileId)
            .ToListAsync();
    }

    public async Task<Guid> GetIdReceiver(Guid fileId)
    {
        var operation = await _context.Operations
            .FirstOrDefaultAsync(op => op.FileId == fileId);
        if (operation == null)
        {
            throw new KeyNotFoundException("Operation not found");
        }

        return operation.ReceiverId;
    }

    public async Task<List<Guid>> GetFileIdsBySender(Guid senderId)
    {
        return await _context.Operations
            .Where(op => op.SenderId == senderId)
            .Select(op => op.FileId)
            .ToListAsync();
    }
}