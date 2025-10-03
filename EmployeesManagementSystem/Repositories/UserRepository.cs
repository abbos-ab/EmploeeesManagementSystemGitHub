using EmployeesManagementSystem.Contexts;
using EmployeesManagementSystem.DTOs;
using EmployeesManagementSystem.Models;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace EmployeesManagementSystem.Repositories
{
    public class UserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }


        //public async Task<UserFile> GetUserFileAsync(Guid id)
        //{
        //    return await _context.UserFiles.FindAsync(id);
        //}
        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task SaveFileAsync(Files file)
        {
            _context.File.Add(file);
            await _context.SaveChangesAsync();
        }

        public async Task<Files> downloasFile(Guid id)
        {
            return await _context.File.FindAsync(id);
        }




    }
}
