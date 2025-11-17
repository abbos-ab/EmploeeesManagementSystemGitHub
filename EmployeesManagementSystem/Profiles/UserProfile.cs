using AutoMapper;
using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Models;

namespace EmployeesManagementSystem.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserRequest, User>();
        CreateMap<CreateUserRequest, User>();
        CreateMap<User, UserResponse>();
        CreateMap<UserResponse, User>();
        CreateMap<OperationList, OperationListResponse>();
        CreateMap<OperationListResponse, OperationList>();

        CreateMap<OperationList, DocumentResponse>();
        CreateMap<DocumentResponse, OperationList>();

        CreateMap<Document, FileResponse>();
        CreateMap<FileResponse, Document>();
    }
}