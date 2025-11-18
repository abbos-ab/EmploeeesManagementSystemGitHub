using AutoMapper;
using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Models;
using EmployeesManagementSystem.Repositories.Interfaces;
using EmployeesManagementSystem.Services;
using EmployeesManagementSystem.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace EmployeesManagementSystem.Tests.Services;

public class DocumentServiceTests
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDocumentRepository _documentRepository;
    private readonly IOperationRepository _operationRepository;
    private readonly IPdfWatermarkService _watermarkService;
    private readonly IMapper _mapper;
    private readonly DocumentService _sut;

    public DocumentServiceTests()
    {
        _currentUserService = Substitute.For<ICurrentUserService>();
        _documentRepository = Substitute.For<IDocumentRepository>();
        _operationRepository = Substitute.For<IOperationRepository>();
        _watermarkService = Substitute.For<IPdfWatermarkService>();
        _mapper = Substitute.For<IMapper>();

        _sut = new DocumentService(
            _documentRepository,
            _mapper,
            _operationRepository,
            _watermarkService,
            _currentUserService);
    }

    [Fact]
    public async Task GetAll_ShouldReturnListOfDocuments()
    {
        // Arrange
        var documents = new List<Document>
        {
            new() { Id = Guid.NewGuid(), Name = "Doc1.pdf", Content = "application/pdf", Data = new byte[100] },
            new() { Id = Guid.NewGuid(), Name = "Doc2.pdf", Content = "application/pdf", Data = new byte[100] }
        };

        _documentRepository.GetAll().Returns(documents);

        // Act
        var result = await _sut.GetAll();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        await _documentRepository.Received(1).GetAll();
    }

    [Fact]
    public async Task GetBySenderId_ShouldReturnDocumentsSentByCurrentUser()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var fileIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var documents = new List<Document>
        {
            new() { Id = fileIds[0], Name = "Doc1.pdf", Content = "application/pdf", Data = new byte[100] },
            new() { Id = fileIds[1], Name = "Doc2.pdf", Content = "application/pdf", Data = new byte[100] }
        };

        var fileResponses = new List<FileResponse>
        {
            new() { Name = "Doc1.pdf" },
            new() { Name = "Doc2.pdf" }
        };

        _currentUserService.GetUserIdFromToken().Returns(senderId);
        _operationRepository.GetFileIdsBySender(senderId).Returns(fileIds);
        _documentRepository.GetByFileIds(fileIds).Returns(documents);
        _mapper.Map<List<FileResponse>>(documents).Returns(fileResponses);

        // Act
        var result = await _sut.GetBySenderId();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        _currentUserService.Received(1).GetUserIdFromToken();
        await _operationRepository.Received(1).GetFileIdsBySender(senderId);
    }

    [Fact]
    public async Task GetByReceiverId_ShouldReturnDocumentsReceivedByCurrentUser()
    {
        // Arrange
        var receiverId = Guid.NewGuid();
        var fileIds = new List<Guid> { Guid.NewGuid() };
        var documents = new List<Document>
        {
            new() { Id = fileIds[0], Name = "Doc1.pdf", Content = "application/pdf", Data = new byte[100] }
        };

        var fileResponses = new List<FileResponse>
        {
            new() { Name = "Doc1.pdf" }
        };

        _currentUserService.GetUserIdFromToken().Returns(receiverId);
        _operationRepository.GetFileIdReceivers(receiverId).Returns(fileIds);
        _documentRepository.GetByReceiverId(fileIds).Returns(documents);
        _mapper.Map<List<FileResponse>>(documents).Returns(fileResponses);

        // Act
        var result = await _sut.GetByReceiverId();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        _currentUserService.Received(1).GetUserIdFromToken();
        await _operationRepository.Received(1).GetFileIdReceivers(receiverId);
    }

    [Fact]
    public async Task UpLoad_ShouldReturnFalse_WhenFileIsNull()
    {
        // Arrange
        var request = new DocumentRequest { FormFile = null };

        // Act
        var result = await _sut.UpLoad(request);

        // Assert
        result.Should().BeFalse();
        await _documentRepository.DidNotReceive().SaveFile(Arg.Any<Document>());
    }

    [Fact]
    public async Task UpLoad_ShouldReturnTrue_WhenFileIsValid()
    {
        // Arrange
        var formFile = Substitute.For<IFormFile>();
        formFile.FileName.Returns("test.pdf");
        formFile.ContentType.Returns("application/pdf");
        formFile.Length.Returns(1000);
        formFile.CopyToAsync(Arg.Any<Stream>()).Returns(Task.CompletedTask);

        var request = new DocumentRequest { FormFile = formFile };

        // Act
        var result = await _sut.UpLoad(request);

        // Assert
        result.Should().BeTrue();
        await _documentRepository.Received(1).SaveFile(Arg.Is<Document>(d =>
            d.Name == "test.pdf" &&
            d.Content == "application/pdf"));
    }

    [Fact]
    public async Task Send_ShouldReturnFalse_WhenSendingToSelf()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fileId = Guid.NewGuid();

        _currentUserService.GetUserIdFromToken().Returns(userId);

        // Act
        var result = await _sut.Send(fileId, userId);

        // Assert
        result.Should().BeFalse();
        await _operationRepository.DidNotReceive().SaveOperation(Arg.Any<OperationList>());
    }

    [Fact]
    public async Task Send_ShouldReturnTrue_WhenSendingToOtherUser()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var receiverId = Guid.NewGuid();
        var fileId = Guid.NewGuid();

        _currentUserService.GetUserIdFromToken().Returns(senderId);

        // Act
        var result = await _sut.Send(fileId, receiverId);

        // Assert
        result.Should().BeTrue();
        await _operationRepository.Received(1).SaveOperation(Arg.Is<OperationList>(o =>
            o.FileId == fileId &&
            o.SenderId == senderId &&
            o.ReceiverId == receiverId &&
            o.ActionType == "Send"));
    }

    [Fact]
    public async Task Download_ShouldReturnDocument_WhenUserHasAccess()
    {
        // Arrange
        var receiverId = Guid.NewGuid();
        var senderId = Guid.NewGuid();
        var fileId = Guid.NewGuid();
        var document = new Document
        {
            Id = fileId,
            Name = "test.pdf",
            Content = "application/pdf",
            Data = new byte[100]
        };

        _currentUserService.GetUserIdFromToken().Returns(receiverId);
        _operationRepository.GetSenderId(fileId).Returns(senderId);
        _documentRepository.Download(fileId, receiverId).Returns(document);

        // Act
        var result = await _sut.Download(fileId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(fileId);
        await _operationRepository.Received(1).SaveOperation(Arg.Is<OperationList>(o =>
            o.ActionType == "Download" &&
            o.FileId == fileId));
    }

    [Fact]
    public async Task Download_ShouldReturnNull_WhenUserDoesNotHaveAccess()
    {
        // Arrange
        var receiverId = Guid.NewGuid();
        var fileId = Guid.NewGuid();

        _currentUserService.GetUserIdFromToken().Returns(receiverId);
        _documentRepository.Download(fileId, receiverId).Returns((Document)null);

        // Act
        var result = await _sut.Download(fileId);

        // Assert
        result.Should().BeNull();
        await _operationRepository.DidNotReceive().SaveOperation(Arg.Any<OperationList>());
    }

    [Fact]
    public void GetContentType_ShouldReturnCorrectMimeType_ForPdf()
    {
        // Act
        var result = _sut.GetContentType("document.pdf");

        // Assert
        result.Should().Be("application/pdf");
    }

    [Fact]
    public void GetContentType_ShouldReturnCorrectMimeType_ForDocx()
    {
        // Act
        var result = _sut.GetContentType("document.docx");

        // Assert
        result.Should().Be("application/vnd.openxmlformats-officedocument.wordprocessingml.document");
    }

    [Fact]
    public void GetContentType_ShouldReturnDefaultMimeType_ForUnknownExtension()
    {
        // Act
        var result = _sut.GetContentType("document.unknown");

        // Assert
        result.Should().Be("application/octet-stream");
    }

    [Fact]
    public async Task StampFile_ShouldThrowException_WhenDocumentNotFound()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var receiverId = Guid.NewGuid();

        _currentUserService.GetUserIdFromToken().Returns(receiverId);
        _operationRepository.GetSenderId(fileId).Returns(Guid.NewGuid());
        _documentRepository.DownloadById(fileId, receiverId)
            .Returns(Task.FromException<Document>(new KeyNotFoundException()));

        // Act
        Func<Task> act = async () => await _sut.StampFile(fileId);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetBySenderId_ShouldReturnEmptyList_WhenNoFilesFound()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        _currentUserService.GetUserIdFromToken().Returns(senderId);
        _operationRepository.GetFileIdsBySender(senderId).Returns(new List<Guid>());

        // Act
        var result = await _sut.GetBySenderId();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByReceiverId_ShouldReturnEmptyList_WhenNoFilesReceived()
    {
        // Arrange
        var receiverId = Guid.NewGuid();
        _currentUserService.GetUserIdFromToken().Returns(receiverId);
        _operationRepository.GetFileIdReceivers(receiverId).Returns(new List<Guid>());

        // Act
        var result = await _sut.GetByReceiverId();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpLoad_ShouldReturnFalse_WhenFileIsEmpty()
    {
        // Arrange
        var formFile = Substitute.For<IFormFile>();
        formFile.Length.Returns(0);
        var request = new DocumentRequest { FormFile = formFile };

        // Act
        var result = await _sut.UpLoad(request);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Download_ShouldReturnNull_WhenDocumentDataIsNull()
    {
        // Arrange
        var receiverId = Guid.NewGuid();
        var senderId = Guid.NewGuid();
        var fileId = Guid.NewGuid();
        var document = new Document
        {
            Id = fileId,
            Name = "test.pdf",
            Content = "application/pdf",
            Data = null!
        };

        _currentUserService.GetUserIdFromToken().Returns(receiverId);
        _operationRepository.GetSenderId(fileId).Returns(senderId);
        _documentRepository.Download(fileId, receiverId).Returns(document);

        // Act
        var result = await _sut.Download(fileId);

        // Assert
        result.Should().BeNull();
        await _operationRepository.Received(1).SaveOperation(Arg.Is<OperationList>(op =>
            op.FileId == fileId &&
            op.ReceiverId == receiverId &&
            op.SenderId == senderId &&
            op.ActionType == "Download"));
    }

    [Theory]
    [InlineData(".jpg", "image/jpeg")]
    [InlineData(".jpeg", "image/jpeg")]
    [InlineData(".png", "image/png")]
    [InlineData(".doc", "application/msword")]
    public void GetContentType_ShouldReturnCorrectMimeTypes(string extension, string expectedMimeType)
    {
        // Act
        var result = _sut.GetContentType($"document{extension}");

        // Assert
        result.Should().Be(expectedMimeType);
    }

    [Fact]
    public async Task Send_ShouldCreateOperationWithCorrectTimestamp()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var receiverId = Guid.NewGuid();
        var fileId = Guid.NewGuid();
        var beforeSend = DateTime.UtcNow;

        _currentUserService.GetUserIdFromToken().Returns(senderId);

        // Act
        await _sut.Send(fileId, receiverId);

        // Assert
        await _operationRepository.Received(1).SaveOperation(Arg.Is<OperationList>(o =>
            o.DateTime >= beforeSend &&
            o.DateTime <= DateTime.UtcNow.AddSeconds(1)));
    }

    [Fact]
    public async Task Download_ShouldNotSaveOperation_WhenDocumentIsNull()
    {
        // Arrange
        var receiverId = Guid.NewGuid();
        var fileId = Guid.NewGuid();

        _currentUserService.GetUserIdFromToken().Returns(receiverId);
        _operationRepository.GetSenderId(fileId).Returns(Guid.NewGuid());
        _documentRepository.Download(fileId, receiverId).Returns((Document)null);

        // Act
        await _sut.Download(fileId);

        // Assert
        await _operationRepository.DidNotReceive().SaveOperation(Arg.Any<OperationList>());
    }

    [Fact]
    public async Task Download_ShouldReturnNull_WhenDocumentDataIsEmpty()
    {
        // Arrange
        var receiverId = Guid.NewGuid();
        var fileId = Guid.NewGuid();
        var document = new Document
        {
            Id = fileId,
            Name = "test.pdf",
            Content = "application/pdf",
            Data = new byte[0]
        };

        _currentUserService.GetUserIdFromToken().Returns(receiverId);
        _operationRepository.GetSenderId(fileId).Returns(Guid.NewGuid());
        _documentRepository.Download(fileId, receiverId).Returns(document);

        // Act
        var result = await _sut.Download(fileId);

        // Assert
        result.Should().BeNull();
        // Note: Operation IS saved before checking data, so we expect 1 call
        await _operationRepository.Received(1).SaveOperation(Arg.Any<OperationList>());
    }

    [Fact]
    public async Task GetBySenderId_ShouldHandleNullDocumentsList()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var fileIds = new List<Guid> { Guid.NewGuid() };

        _currentUserService.GetUserIdFromToken().Returns(senderId);
        _operationRepository.GetFileIdsBySender(senderId).Returns(fileIds);
        _documentRepository.GetByFileIds(fileIds).Returns((List<Document>)null);

        // Act
        var result = await _sut.GetBySenderId();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByReceiverId_ShouldHandleNullDocumentsList()
    {
        // Arrange
        var receiverId = Guid.NewGuid();
        var fileIds = new List<Guid> { Guid.NewGuid() };

        _currentUserService.GetUserIdFromToken().Returns(receiverId);
        _operationRepository.GetFileIdReceivers(receiverId).Returns(fileIds);
        _documentRepository.GetByReceiverId(fileIds).Returns((List<Document>)null);

        // Act
        var result = await _sut.GetByReceiverId();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpLoad_ShouldGenerateUniqueGuidForEachDocument()
    {
        // Arrange
        var formFile1 = Substitute.For<IFormFile>();
        formFile1.FileName.Returns("file1.pdf");
        formFile1.ContentType.Returns("application/pdf");
        formFile1.Length.Returns(1000);
        formFile1.CopyToAsync(Arg.Any<Stream>()).Returns(Task.CompletedTask);

        var formFile2 = Substitute.For<IFormFile>();
        formFile2.FileName.Returns("file2.pdf");
        formFile2.ContentType.Returns("application/pdf");
        formFile2.Length.Returns(1000);
        formFile2.CopyToAsync(Arg.Any<Stream>()).Returns(Task.CompletedTask);

        var capturedIds = new List<Guid>();
        await _documentRepository.SaveFile(Arg.Do<Document>(d => capturedIds.Add(d.Id)));

        // Act
        await _sut.UpLoad(new DocumentRequest { FormFile = formFile1 });
        await _sut.UpLoad(new DocumentRequest { FormFile = formFile2 });

        // Assert
        capturedIds.Should().HaveCount(2);
        capturedIds[0].Should().NotBe(capturedIds[1]);
    }

    [Fact]
    public async Task UpLoad_ShouldStoreCorrectFileData()
    {
        // Arrange
        var fileContent = new byte[] { 1, 2, 3, 4, 5 };
        var formFile = Substitute.For<IFormFile>();
        formFile.FileName.Returns("test.pdf");
        formFile.ContentType.Returns("application/pdf");
        formFile.Length.Returns(fileContent.Length);
        formFile.CopyToAsync(Arg.Any<Stream>()).Returns(async callInfo =>
        {
            var stream = callInfo.Arg<Stream>();
            await stream.WriteAsync(fileContent, 0, fileContent.Length);
        });

        Document? capturedDocument = null;
        await _documentRepository.SaveFile(Arg.Do<Document>(d => capturedDocument = d));

        var request = new DocumentRequest { FormFile = formFile };

        // Act
        var result = await _sut.UpLoad(request);

        // Assert
        result.Should().BeTrue();
        capturedDocument.Should().NotBeNull();
        capturedDocument!.Name.Should().Be("test.pdf");
        capturedDocument.Content.Should().Be("application/pdf");
        capturedDocument.Data.Should().Equal(fileContent);
    }

    [Fact]
    public async Task Send_ShouldNotSaveOperation_WhenReceiverIsSameSender()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _currentUserService.GetUserIdFromToken().Returns(userId);

        // Act
        var result = await _sut.Send(fileId, userId);

        // Assert
        result.Should().BeFalse();
        await _operationRepository.DidNotReceive().SaveOperation(Arg.Any<OperationList>());
    }

    [Fact]
    public async Task GetContentType_ShouldReturnCorrectMimeType_ForAllSupportedFormats()
    {
        // Arrange & Act & Assert
        _sut.GetContentType("document.pdf").Should().Be("application/pdf");
        _sut.GetContentType("document.doc").Should().Be("application/msword");
        _sut.GetContentType("document.docx").Should().Be("application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        _sut.GetContentType("image.png").Should().Be("image/png");
        _sut.GetContentType("image.jpg").Should().Be("image/jpeg");
        _sut.GetContentType("image.jpeg").Should().Be("image/jpeg");
        _sut.GetContentType("file.xyz").Should().Be("application/octet-stream");
    }

    [Fact]
    public async Task GetAll_ShouldMapDocumentsCorrectly()
    {
        // Arrange
        var documents = new List<Document>
        {
            new() { Id = Guid.NewGuid(), Name = "doc1.pdf", Content = "application/pdf", Data = new byte[] { 1, 2, 3 } },
            new() { Id = Guid.NewGuid(), Name = "doc2.docx", Content = "application/docx", Data = new byte[] { 4, 5, 6 } }
        };

        _documentRepository.GetAll().Returns(documents);

        // Act
        var result = await _sut.GetAll();

        // Assert
        result.Should().HaveCount(2);
        result[0].Id.Should().Be(documents[0].Id);
        result[0].Name.Should().Be("doc1.pdf");
        result[0].Content.Should().Be("application/pdf");
        result[1].Id.Should().Be(documents[1].Id);
        result[1].Name.Should().Be("doc2.docx");
    }

    [Fact]
    public async Task Download_ShouldSaveOperationWithCorrectActionType()
    {
        // Arrange
        var receiverId = Guid.NewGuid();
        var senderId = Guid.NewGuid();
        var fileId = Guid.NewGuid();
        var document = new Document
        {
            Id = fileId,
            Name = "test.pdf",
            Content = "application/pdf",
            Data = new byte[] { 1, 2, 3 }
        };

        _currentUserService.GetUserIdFromToken().Returns(receiverId);
        _operationRepository.GetSenderId(fileId).Returns(senderId);
        _documentRepository.Download(fileId, receiverId).Returns(document);

        OperationList? capturedOperation = null;
        await _operationRepository.SaveOperation(Arg.Do<OperationList>(op => capturedOperation = op));

        // Act
        var result = await _sut.Download(fileId);

        // Assert
        result.Should().NotBeNull();
        capturedOperation.Should().NotBeNull();
        capturedOperation!.ActionType.Should().Be("Download");
        capturedOperation.FileId.Should().Be(fileId);
        capturedOperation.SenderId.Should().Be(senderId);
        capturedOperation.ReceiverId.Should().Be(receiverId);
    }
}