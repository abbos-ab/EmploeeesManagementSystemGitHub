using AutoMapper;
using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Models;

namespace EmployeesManagementSystem.Profiles;

public class DepartmentProfile : Profile
{
    public DepartmentProfile()
    {
        // Department mappings
        CreateMap<Department, DepartmentResponse>();
    }
}