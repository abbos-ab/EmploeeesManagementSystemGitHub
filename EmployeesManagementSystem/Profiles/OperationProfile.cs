using AutoMapper;
using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Models;

namespace EmployeesManagementSystem.Profiles;

public class OperationProfile : Profile
{
    public OperationProfile()
    {
        // OperationList mappings
        CreateMap<OperationList, OperationListResponse>();
    }
}

