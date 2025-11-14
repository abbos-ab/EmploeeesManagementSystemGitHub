using AutoMapper;
using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Repositories;
using Org.BouncyCastle.Security;

namespace EmployeesManagementSystem.Services
{
    public class AssignmentsService
    {
        public readonly AssignmentsRepository _repository;
        public readonly IMapper _mapper;
        public AssignmentsService(AssignmentsRepository departmentRepository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = departmentRepository;
        }

        public async Task<List<AssignmentsResponce>> GetAll()
        {
            var departments = await _repository.GetAll();
            var responce = _mapper.Map<List<AssignmentsResponce>>(departments);
            return responce;
        }

        public async Task<AssignmentsResponce> Create(AssignmentsRequest createDepartment)
        {
            var existAssignment = await _repository
                .GetForCheck(createDepartment.IdUser,
                createDepartment.IdDeportment,
                createDepartment.IdRole);

            var IdRole = createDepartment.IdRole.ToString();

            Guid a = Guid.Parse(IdRole);
            Guid b = Guid.Parse("11111111-1111-1111-1111-111111111111");

            if (a == b)
                throw new InvalidParameterException("You cannot assign the 'Employee' role using RoteAdmin");

            if (existAssignment is not null)
                throw new InvalidParameterException("This assignment already exists.");

            var createdDepartment = await _repository.Add(createDepartment);
            var result = _mapper.Map<AssignmentsResponce>(createdDepartment);
            return result;
        }

        public async Task Delete(Guid id)
        {
            await _repository.Delete(id);
        }


        public async Task<List<AssignmentsResponce>> GetUserAssignmentsAsync(Guid userId)
        {
            var records = await _repository.GetByUserIdAsync(userId);

            var responce = records.Select(r => new AssignmentsResponce
            {
                IdUser = r.IdUser,
                IdDeportment = r.IdDeportment,
                IdRole = r.IdRole
            }).ToList();

            return responce;
        }
    }
}
