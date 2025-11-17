using AutoMapper;
using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Models;

namespace EmployeesManagementSystem.Profiles;

public class DepartmentProfile : Profile
{
    public DepartmentProfile()
    {
        CreateMap<AssignmentsRequest, UserDepartmentRole>();
        CreateMap<UserDepartmentRole, AssignmentsResponse>();

        CreateMap<Department, DepartmentResponse>();
        CreateMap<DepartmentResponse, Department>();
    }
}