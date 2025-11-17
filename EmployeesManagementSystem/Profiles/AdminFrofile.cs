using AutoMapper;
using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Models;

namespace EmployeesManagementSystem.Profiles;

public class AdminProfile : Profile
{
    public AdminProfile()
    {
        CreateMap<CreateUserRequest, User>();
        CreateMap<User, UserResponse>();
        CreateMap<CreateUserRequest, UserResponse>();
        CreateMap<DocumentResponse, Document>();
        CreateMap<Document, DocumentResponse>();
    }
}