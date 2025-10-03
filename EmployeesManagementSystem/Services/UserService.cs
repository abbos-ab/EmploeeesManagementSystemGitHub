using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Models;
using EmployeesManagementSystem.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EmployeesManagementSystem.Services
{
    public class UserService
    {
        private readonly UserRepository _repository;
        public UserService(UserRepository repository)
        {
            _repository = repository;
        }
        public async Task<bool> SendFileAsynce(SendFileRequest request)
        {
            var receiver = await _repository.GetByIdAsync(request.ReceiverId);
            if (receiver == null)
                return false;

            byte[] fileData;
            using (var ms = new MemoryStream())
            {
                await request.formFile.CopyToAsync(ms);
                fileData = ms.ToArray();
            }

            var file = new Files
            {
                Id = Guid.NewGuid(),
                Name = request.formFile.FileName,
                Content = request.formFile.ContentType,
                ReceiverId = request.ReceiverId,
                Data = fileData,
                CreatedBy = Guid.Parse("08ddfe9f-e22c-4056-8196-ae42978dfec2"),
                CreatedAt = DateTime.UtcNow
            };
            await _repository.SaveFileAsync(file);
            return true;
        }
        public async Task<Files?> DownloadAsync(Guid id)
        {
            return await _repository.downloasFile(id);
        }
    }
}

