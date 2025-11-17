using AutoMapper;
using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Models;
using EmployeesManagementSystem.Repositories;

namespace EmployeesManagementSystem.Services;

public class DocumentService
{
    private readonly CurrentUserService _currentUserService;
    private readonly DocumentRepository _documentRepository;
    private readonly OperationRepository _operationRepository;
    private readonly PdfWatermarkService _watermarkService;
    private readonly IMapper _mapper;

    public DocumentService(
        DocumentRepository repository,
        IMapper mapper,
        OperationRepository operationRepository,
        PdfWatermarkService pdfWatermarkService,
        CurrentUserService currentUserService)
    {
        _mapper = mapper;
        _documentRepository = repository;
        _operationRepository = operationRepository;
        _watermarkService = pdfWatermarkService;
        _currentUserService = currentUserService;
    }

    public async Task<List<DocumentResponse>> GetAll()
    {
        var documents = await _documentRepository.GetAll();
        var response = documents.Select(doc => new DocumentResponse
        {
            Id = doc.Id,
            Name = doc.Name,
            Content = doc.Content
        }).ToList();
        return response;
    }

    public async Task<List<FileResponse>> GetBySenderId()
    {
        var senderId = _currentUserService.GetUserIdFromToken();
        var fileIds = await _operationRepository.GetFileIdsBySender(senderId);
        if (fileIds == null || !fileIds.Any())
            return new List<FileResponse>();

        var documents = await _documentRepository.GetByFileIds(fileIds);
        if (documents == null || !documents.Any())
            return new List<FileResponse>();

        var responses = _mapper.Map<List<FileResponse>>(documents);
        if (responses == null || !responses.Any())
            return new List<FileResponse>();
        return responses;
    }

    public async Task<List<FileResponse>> GetByReceiverId()
    {
        var receiverId = _currentUserService.GetUserIdFromToken();

        var fileIds = await _operationRepository.GetFileIdReceivers(receiverId);
        if (fileIds == null || !fileIds.Any())
            return new List<FileResponse>();
        var document = await _documentRepository.GetByReceiverId(fileIds);
        if (document == null || !document.Any())
            return new List<FileResponse>();
        var responses = _mapper.Map<List<FileResponse>>(document);

        if (responses == null || !responses.Any())
            return new List<FileResponse>();

        return responses;
    }

    public async Task<bool> UpLoad(DocumentRequest request)
    {
        if (request.FormFile == null || request.FormFile.Length == 0)
            return false;

        byte[] fileData;
        using (var ms = new MemoryStream())
        {
            await request.FormFile.CopyToAsync(ms);
            fileData = ms.ToArray();
        }

        var file = new Document
        {
            Id = Guid.NewGuid(),
            Name = request.FormFile.FileName,
            Content = request.FormFile.ContentType,
            Data = fileData,
        };
        await _documentRepository.SaveFile(file);
        return true;
    }

    public async Task<bool> Send(Guid id, Guid receiverUserId)
    {
        var senderId = _currentUserService.GetUserIdFromToken();
        var operation = new OperationList
        {
            Id = Guid.NewGuid(),
            FileId = id,
            SenderId = senderId,
            ReceiverId = receiverUserId,
            ActionType = "Send",
            DateTime = DateTime.UtcNow
        };
        if (receiverUserId != senderId)
        {
            await _operationRepository.SaveOperation(operation);
            return true;
        }

        return false;
    }

    public async Task<Document> Download(Guid id)
    {
        var receiverId = _currentUserService.GetUserIdFromToken();
        var sender = _operationRepository.GetSenderId(id);
        var senderId = await sender;
        var result = await _documentRepository.Download(id, receiverId);
        if (result != null)
        {
            var operation = new OperationList
            {
                FileId = id,
                ReceiverId = receiverId,
                SenderId = senderId,
                ActionType = "Download",
                DateTime = DateTime.UtcNow
            };
            await _operationRepository.SaveOperation(operation);

            if (result.Data == null || result.Data.Length == 0)
                return null;
            return result;
        }

        return null;
    }

    public async Task<Document> StampFile(Guid fileId)
    {
        var receiverId = _currentUserService.GetUserIdFromToken();
        var senderId = await _operationRepository.GetSenderId(fileId);
        var result = await _documentRepository.DownloadById(fileId, receiverId);
        
        if (result == null)
            throw new KeyNotFoundException("Document not found.");
        
        if (result.Data == null || result.Data.Length == 0)
            throw new InvalidOperationException("Document data is empty.");
        
        if (string.IsNullOrEmpty(result.Content))
            throw new InvalidOperationException("Document content type is missing.");
        result.Content = Convert.ToBase64String(result.Data);

        var fileExtension = Path.GetExtension(result.Name).ToLower();
        var fileBytes = Convert.FromBase64String(result.Content);
        byte[] stampedPdf;
        if (fileExtension == ".pdf")
            stampedPdf = _watermarkService.AddStampToLastPage(fileBytes, "CONFIRMED");

        else if (fileExtension == ".docx" || fileExtension == ".doc")
            stampedPdf = _watermarkService.AddStampToLastPage(ConvertDocxToPdf(fileBytes), "CONFIRMED");
        else
            throw new NotSupportedException("File format not supported for stamping.");

        var newFileId = Guid.NewGuid();

        var operation = new OperationList
        {
            FileId = newFileId,
            ReceiverId = receiverId,
            SenderId = senderId,
            ActionType = "stamped",
            DateTime = DateTime.UtcNow
        };
        result.Id = newFileId;
        result.Data = stampedPdf;
        await _documentRepository.InsertFile(result);
        await _operationRepository.SaveOperation(operation);
        return result;
    }

    public byte[] ConvertDocxToPdf(byte[] docxBytes)
    {
        using var ms = new MemoryStream(docxBytes);
        var doc = new Aspose.Words.Document(ms);
        using var output = new MemoryStream();
        doc.Save(output, Aspose.Words.SaveFormat.Pdf);
        return output.ToArray();
    }

    public Guid GetSenderIdByDocumentId(Guid documentId)
    {
        var senderIdTask = _operationRepository.GetSenderId(documentId);
        senderIdTask.Wait();
        return senderIdTask.Result;
    }

    public string GetContentType(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            _ => "application/octet-stream"
        };
    }
}