using AutoMapper;
using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Models;
using EmployeesManagementSystem.Repositories;
namespace EmployeesManagementSystem.Services
{
    public class DepartmentService
    {
        public readonly DepartmentRepository _repository;
        public readonly IMapper _mapper;
        public DepartmentService(DepartmentRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }
        
        public async Task<List<DepartamentResponce>> GetAll()
        {
            var departments = await _repository.GetAll();
            var responce = _mapper.Map<List<DepartamentResponce>>(departments);
            return responce;
        }


        public async Task<DepartamentResponce> GetById(Guid id)
        {
            var department = await _repository.GetById(id);
            var result = _mapper.Map<DepartamentResponce>(department);
            return result;
        }

        public async Task<DepartamentResponce> Create(string name)
        {
            var newDepartment = new Departament
            {
                Id = Guid.NewGuid(),
                Name = name
            };
            var createdDepartment = await _repository.Add(newDepartment);
            var result = _mapper.Map<DepartamentResponce>(createdDepartment);
            return result;
        }

        public async Task<DepartamentResponce> Update(Guid id, string name)
        {
            var department = await _repository.GetById(id);
            if (department == null)
                return null;
            department.Name = name;
            var updatedDepartment = await _repository.Update(department);
            var result = _mapper.Map<DepartamentResponce>(updatedDepartment);
            return result;
        }

        public async Task Delete(Guid id)
        {
            var department = await _repository.GetById(id);
            await _repository.Delete(department);
        }

    }
}
