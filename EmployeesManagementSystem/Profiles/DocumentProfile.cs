using AutoMapper;
using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Models;

namespace EmployeesManagementSystem.Profiles;

public class DocumentProfile : Profile
{
    public DocumentProfile()
    {
        // Document mappings
        CreateMap<Document, DocumentResponse>();

        // FileResponse mappings
        CreateMap<Document, FileResponse>();
    }
}