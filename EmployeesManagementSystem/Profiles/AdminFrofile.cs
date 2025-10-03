using AutoMapper;
using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Models;
namespace EmployeesManagementSystem.Profiles
{
    public class AdminProfile : Profile
    {
        public AdminProfile()
        {
            CreateMap<CreateAdminRequest, User>();
            CreateMap<User, AdminResponce>();
            CreateMap<CreateAdminRequest, AdminResponce>();
            CreateMap<FileResponce, Files>();
            CreateMap<Files, FileResponce>();            
        }
    }
}
