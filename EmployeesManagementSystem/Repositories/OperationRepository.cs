using Microsoft.EntityFrameworkCore;
using EmployeesManagementSystem.Contexts;
using EmployeesManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeesManagementSystem.Repositories
{
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
                .FirstOrDefaultAsync(op => op.FileID == fileId);
            if (operation == null)
            {
                throw new KeyNotFoundException("Operation not found.");
            }
            return operation.SenderId;
        }

        public async Task<List<Guid>> GetFileIdRecivers(Guid reciverId)
        {
            return await _context.Operations
                .Where(op => op.ReceiverId == reciverId)
                .Select(op => op.FileID)
                .ToListAsync();
        }

        public async Task<Guid> GetIdReciver(Guid idFile)
        {
            var operation = await _context.Operations
                .FirstOrDefaultAsync(op => op.FileID == idFile);
            if (operation == null)
            {
                throw new KeyNotFoundException("operator not found");
            }

            return operation.ReceiverId;
        }

        public async Task<List<Guid>> GetFileIdsBySender(Guid senderId)
        {
            return await _context.Operations
                .Where(op => op.SenderId == senderId)
                .Select(op => op.FileID)
                .ToListAsync();
        }
    }
}
