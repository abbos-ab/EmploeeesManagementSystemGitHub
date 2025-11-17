using AutoMapper;
using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Models;

namespace EmployeesManagementSystem.Profiles;

public class RoleProfile : Profile
{
    public RoleProfile()
    {
        // Role mappings
        CreateMap<Role, RoleResponse>();
    }
}