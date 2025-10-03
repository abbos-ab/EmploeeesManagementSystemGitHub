using AutoMapper;
using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Models;
using EmployeesManagementSystem.Repositories;

namespace EmployeesManagementSystem.Services
{
    public class AdminService
    {
        private readonly AdminRepository _repositories;
        private readonly IMapper _mapper;

        public AdminService(AdminRepository userRepositories, IMapper mapper)
        {
            _repositories = userRepositories;
            _mapper = mapper;
        }

        public async Task<List<AdminResponce>> GetAlL()
        {
            var user = await _repositories.GetAll();
            var responce = _mapper.Map<List<AdminResponce>>(user);
            return responce;
        }

        public async Task<List<FileResponce>> GetAllFiles()
        {
            var file = await _repositories.GetAllFiles();
            var responce = _mapper.Map<List<FileResponce>>(file);
            return responce;
        }

        public async Task<AdminResponce> Create(CreateAdminRequest createUser)
        {

            var user = _mapper.Map<User>(createUser);
            var createdUser = await _repositories.Add(user);
            var responce = _mapper.Map<AdminResponce>(createdUser);

            return responce;
        }

        public async Task Delete(Guid id)
        {
            await _repositories.Delete(id);
        }
    }
}
