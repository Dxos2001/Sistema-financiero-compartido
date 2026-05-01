using FinancieraBackend.Domain.Interfaces;
using FinancieraBackend.Domain.Models;
using FinancieraBackend.Domain.DTOs;
using FinancieraBackend.Infrastructure;

namespace FinancieraBackend.Application.Services
{
    public class UserService : IUserService
    {
        private readonly DBConnection _context;

        public UserService(DBConnection context)
        {
            _context = context;
        }

        public Task<Users> CreateUserAsync(CreateUserDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteUserAsync(string username)
        {
            throw new NotImplementedException();
        }

        public Task<List<Users>> GetAllUsersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Users> GetUserAsync(string username)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateUserAsync(string username, UpdateUserDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UserExistsAsync(string username)
        {
            throw new NotImplementedException();
        }
    }
}
