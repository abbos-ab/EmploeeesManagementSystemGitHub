using AutoMapper;
using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Models;

namespace EmployeesManagementSystem.Profiles;

public class AssignmentsProfile : Profile
{
    public AssignmentsProfile()
    {
        // UserDepartmentRole (Assignments) mappings
        CreateMap<AssignmentsRequest, UserDepartmentRole>();
        CreateMap<UserDepartmentRole, AssignmentsResponse>();
    }
}
