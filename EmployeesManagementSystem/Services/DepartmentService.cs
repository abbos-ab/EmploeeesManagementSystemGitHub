using AutoMapper;
using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Models;
using EmployeesManagementSystem.Repositories;

namespace EmployeesManagementSystem.Services;

public class DepartmentService
{
    private readonly DepartmentRepository _repository;
    private readonly IMapper _mapper;

    public DepartmentService(DepartmentRepository repository, IMapper mapper)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<List<DepartmentResponse>> GetAll()
    {
        var departments = await _repository.GetAll();
        var response = _mapper.Map<List<DepartmentResponse>>(departments);
        return response;
    }


    public async Task<DepartmentResponse> GetById(Guid id)
    {
        var department = await _repository.GetById(id);
        var result = _mapper.Map<DepartmentResponse>(department);
        return result;
    }

    public async Task<DepartmentResponse> Create(string name)
    {
        var newDepartment = new Department
        {
            Id = Guid.NewGuid(),
            Name = name
        };
        var createdDepartment = await _repository.Add(newDepartment);
        var result = _mapper.Map<DepartmentResponse>(createdDepartment);
        return result;
    }

    public async Task<DepartmentResponse> Update(Guid id, string name)
    {
        var department = await _repository.GetById(id);
        if (department == null)
            return null;
        department.Name = name;
        var updatedDepartment = await _repository.Update(department);
        var result = _mapper.Map<DepartmentResponse>(updatedDepartment);
        return result;
    }

    public async Task Delete(Guid id)
    {
        var department = await _repository.GetById(id);
        await _repository.Delete(department);
    }
}