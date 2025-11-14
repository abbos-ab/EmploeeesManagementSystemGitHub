using AutoMapper;
using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Models;

namespace EmployeesManagementSystem.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserRequest, User>();
            CreateMap<CreateUserRequest, User>();
            CreateMap<User, UserResponce>();
            CreateMap<UserResponce, User>();
            CreateMap<OperationList, OperationListResponce>();
            CreateMap<OperationListResponce, OperationList>();

            CreateMap<OperationList, DocumentResponce>();
            CreateMap<DocumentResponce, OperationList>();

            CreateMap<Document, FileResponce>();
            CreateMap<FileResponce, Document>();
        }
    }

}
