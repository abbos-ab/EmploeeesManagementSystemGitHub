using AutoMapper;
using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Repositories;
using Org.BouncyCastle.Security;

namespace EmployeesManagementSystem.Services;

public class AssignmentsService
{
    private readonly AssignmentsRepository _repository;
    private readonly IMapper _mapper;

    public AssignmentsService(AssignmentsRepository repository, IMapper mapper)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<List<AssignmentsResponse>> GetAll()
    {
        var assignments = await _repository.GetAll();
        var response = _mapper.Map<List<AssignmentsResponse>>(assignments);
        return response;
    }

    public async Task<AssignmentsResponse> Create(AssignmentsRequest request)
    {
        var existingAssignment = await _repository
            .GetForCheck(request.UserId,
                request.DepartmentId,
                request.RoleId);

        var roleId = request.RoleId.ToString();

        var requestedRoleId = Guid.Parse(roleId);
        var superAdminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        if (requestedRoleId == superAdminRoleId)
            throw new InvalidParameterException("You cannot assign the 'SuperAdmin' role using this endpoint");

        if (existingAssignment is not null)
            throw new InvalidParameterException("This assignment already exists.");

        var createdAssignment = await _repository.Add(request);
        var result = _mapper.Map<AssignmentsResponse>(createdAssignment);
        return result;
    }

    public async Task Delete(Guid id)
    {
        await _repository.Delete(id);
    }

    public async Task<List<AssignmentsResponse>> GetUserAssignmentsAsync(Guid userId)
    {
        var records = await _repository.GetByUserIdAsync(userId);

        var response = _mapper.Map<List<AssignmentsResponse>>(records);

        return response;
    }
}