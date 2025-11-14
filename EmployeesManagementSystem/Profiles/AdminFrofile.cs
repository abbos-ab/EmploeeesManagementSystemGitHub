using AutoMapper;
using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Models;
namespace EmployeesManagementSystem.Profiles
{
    public class AdminProfile : Profile
    {
        public AdminProfile()
        {
            CreateMap<CreateUserRequest, User>();
            CreateMap<User, UserResponce>();
            CreateMap<CreateUserRequest, UserResponce>();
            CreateMap<DocumentResponce, Document>();
            CreateMap<Document, DocumentResponce>();            
        }
    }
}
