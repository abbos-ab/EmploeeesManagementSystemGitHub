using EmployeesManagementSystem.Contexts;
using EmployeesManagementSystem.Models;
using EmployeesManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeesManagementSystem.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly AppDbContext _context;

    public DocumentRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<Document>> GetAll()
    {
        return _context.Documents.ToListAsync();
    }

    public async Task<List<Document>> GetByFileIds(List<Guid> fileIds)
    {
        return await _context.Documents
            .Where(d => fileIds.Contains(d.Id))
            .ToListAsync();
    }


    public async Task<List<Document>> GetByReceiverId(List<Guid> fileId)
    {
        return await _context.Documents
            .Where(d => fileId.Contains(d.Id))
            .ToListAsync();
    }

    public async Task SaveFile(Document file)
    {
        _context.Documents.Add(file);
        await _context.SaveChangesAsync();
    }

    public async Task InsertFile(Document doc)
    {
        if (doc.Data == null || doc.Data.Length == 0)
            throw new ArgumentException("Document data cannot be null or empty.");

        doc.Content = "Stamped PDF";
        await _context.Documents.AddAsync(doc);
        await _context.SaveChangesAsync();
    }


    public async Task<Document> Download(Guid id, Guid receiverId)
    {
        if (await _context.Operations.AnyAsync(s =>
                s.FileId == id && s.ReceiverId == receiverId || s.FileId == id && s.SenderId == receiverId))
        {
            return await _context.Documents.FirstOrDefaultAsync(s => s.Id == id);
        }

        return null;
    }

    public async Task<Document> DownloadById(Guid id, Guid receiverId)
    {
        if (await _context.Operations.AnyAsync(s => s.FileId == id && s.ReceiverId == receiverId))
        {
            return await _context.Documents.FirstAsync(s => s.Id == id);
        }

        throw new KeyNotFoundException("Document not found for the file Id");
    }
}