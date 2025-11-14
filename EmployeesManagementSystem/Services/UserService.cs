using AutoMapper;
using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Models;
using EmployeesManagementSystem.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EmployeesManagementSystem.Services
{
    public class UserService
    {
        private readonly UserRepository _repository;
        private readonly IMapper _mapper;

        public UserService(UserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<UserResponce>> GetAlL()
        {
            var user = await _repository.GetAll();
            var responce = _mapper.Map<List<UserResponce>>(user);
            return responce;
        }

        public async Task<UserResponce> GetById(Guid id)
        {
            var user = await _repository.GetById(id);
            var responce = _mapper.Map<UserResponce>(user);
            return responce;
        }

        public async Task<UserResponce> Create(CreateUserRequest createUser)
        {

            var existEmile = await _repository.GetByEmailAsync(createUser.Email);
            if (existEmile is not null)
            {
                throw new DbUpdateException($"User with email {createUser.Email} already exists.");
            }

            var user = new User();
            var hashedPassword = new PasswordHasher<User>()
                 .HashPassword(user, createUser.Password);
            user.Name = createUser.Name;
            user.Email = createUser.Email;
            user.Password = hashedPassword;
            var createdUser = await _repository.Add(user);
            var responce = _mapper.Map<UserResponce>(createdUser);
            return responce;
        }
        public async Task<UserResponce> Update(Guid id, UserRequest upDateUser)
        {
            var user = _mapper.Map<User>(upDateUser);
            user.Id = id;
            var updatedUser = await _repository.Update(user);
            var responce = _mapper.Map<UserResponce>(updatedUser);
            return responce;
        }

        public async Task Delete(Guid id)
        {
            await _repository.Delete(id);
        }
    }
}
