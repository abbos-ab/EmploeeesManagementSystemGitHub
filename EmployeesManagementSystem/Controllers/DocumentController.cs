using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using EmployeesManagementSystem.Repositories;
namespace EmployeesManagementSystem.Controllers
{
    public class DocumentController : BaseController
    {
        public readonly DocumentService _documentService;
        public readonly CurrentUserService _currentUserService;
        public readonly PdfWatermarkService _watermarkService;
        public readonly DocumentRepository _documentRepository;
        public readonly IMapper _mapper;
        public DocumentController(DocumentService documentService, CurrentUserService currentUserService, PdfWatermarkService watermarkService,DocumentRepository documentRepository,IMapper mapper)
        {
            _documentService = documentService;
            _currentUserService = currentUserService;
            _watermarkService = watermarkService;
            _documentRepository = documentRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public Task<List<DocumentResponce>> GetAll()
        {
            return _documentService.GetAll();
        }

        [HttpGet]
        public async Task<ActionResult<List<FileResponce>>> GetBySenderId()
        {
            var result = await _documentService.GetBySenderId();
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<FileResponce>>> GetByReceiverId()
        {
            var result = await _documentService.GetByReceiverId();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> UpLoad([FromForm] DocumentRequest request)
        {
            var success = await _documentService.UpLoad(request);
            if (!success)
                return NotFound("File not fined");

            return Ok("File Successfully sended");
        }

        [HttpGet]
        public async Task<IActionResult> Send(Guid id, Guid receiverUserId)
        {
            var success = await _documentService.Send(id, receiverUserId);
            if (!success)
                return NotFound("User or document not fined");
            return Ok("File Successfully sended");
        }

        [HttpGet]
        public async Task<IActionResult> Download(Guid id)
        {
            var doc = await _documentService.Download(id);
            if(doc == null)
                return NotFound("File not fined");
            var contentType =_documentService.GetContentType(doc.Name);
            return File(doc.Data, contentType, doc.Name);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadStampedPdf(Guid id)
        {
            await _documentService.StempFile(id);
            return Ok("File successful stemped and sended");
        }
    }
}
