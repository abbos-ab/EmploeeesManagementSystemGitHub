using AutoMapper;
using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Models;
using EmployeesManagementSystem.Repositories;

namespace EmployeesManagementSystem.Services
{
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

        public async Task<List<DocumentResponce>> GetAll()
        {
            var documents = await _documentRepository.GetAll();
            var responce = documents.Select(doc => new DocumentResponce
            {
                Id = doc.Id,
                Name = doc.Name,
                Content = doc.Content
            }).ToList();
            return responce;
        }

        public async Task<List<FileResponce>> GetBySenderId()
        {
            var senderId = _currentUserService.GetUserIdFromToken();
            var fileIds = await _operationRepository.GetFileIdsBySender(senderId);
            if (fileIds == null || !fileIds.Any())
                return new List<FileResponce>();

            var documents = await _documentRepository.GetByFileIds(fileIds);
            if (documents == null || !documents.Any())
                return new List<FileResponce>();

            var responses = _mapper.Map<List<FileResponce>>(documents);
            if (responses == null || !responses.Any())
                return new List<FileResponce>();
            return responses;
        }

        public async Task<List<FileResponce>> GetByReceiverId()
        {
            var receiverId = _currentUserService.GetUserIdFromToken();

            var fileIds = await _operationRepository.GetFileIdRecivers(receiverId);
            if (fileIds == null || !fileIds.Any())
                return new List<FileResponce>();
            var document = await _documentRepository.GetByReceiverId(fileIds);
            if (document == null || !document.Any())
                return new List<FileResponce>();
            var responses = _mapper.Map<List<FileResponce>>(document);

            if (responses == null || !responses.Any())
                return new List<FileResponce>();

            return responses;
        }

        public async Task<bool> UpLoad(DocumentRequest request)
        {
            if (request.formFile == null || request.formFile.Length == 0)
                return false;

            byte[] fileData;
            using (var ms = new MemoryStream())
            {
                await request.formFile.CopyToAsync(ms);
                fileData = ms.ToArray();
            }

            var file = new Document
            {
                Id = Guid.NewGuid(),
                Name = request.formFile.FileName,
                Content = request.formFile.ContentType,
                Data = fileData,
            };
            await _documentRepository.SaveFile(file);
            return true;
        }

        public async Task<bool> Send(Guid id, Guid receiverUserId)
        {
            Guid senderId = _currentUserService.GetUserIdFromToken();
            var res = false;
            var operation = new OperationList
            {
                Id = Guid.NewGuid(),
                FileID = id,
                SenderId = senderId,
                ReceiverId = receiverUserId,
                AcrionType = "Send",
                DateTime = DateTime.UtcNow
            };
            if (receiverUserId != senderId)
            {
                await _operationRepository.SaveOperation(operation);
                return res = true;
            }
            return res;
        }

        public async Task<Document?> Download(Guid id)
        {
            var receiverId = _currentUserService.GetUserIdFromToken();
            var sender = _operationRepository.GetSenderId(id);
            Guid senderId = await sender;
            var res = await _documentRepository.Download(id, receiverId);
            if (res != null)
            {
                var operation = new OperationList
                {
                    FileID = id,
                    ReceiverId = receiverId,
                    SenderId = senderId,
                    AcrionType = "Download",
                    DateTime = DateTime.UtcNow
                };
                await _operationRepository.SaveOperation(operation);

                if (res.Data == null || res.Data.Length == 0)
                    return null;
                return res;
            }
            return null;
        }
        public async Task<Document> StempFile(Guid idFile)
        {
            Guid receiverId = _currentUserService.GetUserIdFromToken();
            var sender = _operationRepository.GetSenderId(idFile);
            Guid senderId = await sender;
            var res = await _documentRepository.DownloadById(idFile, receiverId);
            if (res == null)
                return null;
            if (res.Data == null || res.Data.Length == 0)
                return null;
            if (string.IsNullOrEmpty(res.Content))
                return null;
            res.Content = Convert.ToBase64String(res.Data);

            string fileExtension = Path.GetExtension(res.Name).ToLower();
            byte[] fileBytes = Convert.FromBase64String(res.Content);
            byte[] stempedPdf;
            if (fileExtension == ".pdf")
                stempedPdf = _watermarkService.AddStampToLastPage(fileBytes, "CONFIRMED");

            else if (fileExtension == ".docx" || fileExtension == ".doc")
                stempedPdf = _watermarkService.AddStampToLastPage(ConvertDocxToPdf(fileBytes), "CONFIRMED");
            else
                throw new NotSupportedException("File format not supported for stamping.");

            var fileId = Guid.NewGuid();

            var operation = new OperationList
            {
                FileID = fileId,
                ReceiverId = receiverId,
                SenderId = senderId,
                AcrionType = "stemped",
                DateTime = DateTime.UtcNow
            };
            res.Id = fileId;
            res.Data = stempedPdf;
            await _documentRepository.IncertFile(res);
            await _operationRepository.SaveOperation(operation);
            return res;
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
}

