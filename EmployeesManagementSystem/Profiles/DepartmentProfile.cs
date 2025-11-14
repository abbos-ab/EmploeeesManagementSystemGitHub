using AutoMapper;
using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Models;
namespace EmployeesManagementSystem.Profiles
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<AssignmentsRequest, UserDeportmentRole>();
            CreateMap<UserDeportmentRole, AssignmentsResponce>();

            CreateMap<Departament, DepartamentResponce>();
            CreateMap<DepartamentResponce, Departament>();
        }
    }
}
