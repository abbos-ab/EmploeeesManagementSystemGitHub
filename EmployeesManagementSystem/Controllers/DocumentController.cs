using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeesManagementSystem.Controllers;

// All authenticated users can access documents
public class DocumentController : BaseController
{
    private readonly IDocumentService _documentService;

    public DocumentController(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    [HttpGet]
    public Task<List<DocumentResponse>> GetAll()
    {
        return _documentService.GetAll();
    }

    [HttpGet]
    public async Task<ActionResult<List<FileResponse>>> GetBySenderId()
    {
        var result = await _documentService.GetBySenderId();
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<FileResponse>>> GetByReceiverId()
    {
        var result = await _documentService.GetByReceiverId();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> UpLoad([FromForm] DocumentRequest request)
    {
        var success = await _documentService.UpLoad(request);
        if (!success)
            return NotFound("File not found");

        return Ok("File successfully uploaded");
    }

    [HttpGet]
    public async Task<IActionResult> Send(Guid id, Guid receiverUserId)
    {
        var success = await _documentService.Send(id, receiverUserId);
        if (!success)
            return NotFound("User or document not found");
        return Ok("File successfully sent");
    }

    [HttpGet]
    public async Task<IActionResult> Download(Guid id)
    {
        var doc = await _documentService.Download(id);
        if (doc == null)
            return NotFound("File not found");
        var contentType = _documentService.GetContentType(doc.Name);
        return File(doc.Data, contentType, doc.Name);
    }

    [HttpGet]
    public async Task<IActionResult> DownloadStampedPdf(Guid id)
    {
        try
        {
            await _documentService.StampFile(id);
            return Ok("File successfully stamped and sent");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (NotSupportedException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}