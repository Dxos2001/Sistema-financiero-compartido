using FinancieraBackend.Domain.Models;
using FinancieraBackend.Domain.DTOs;

namespace FinancieraBackend.Domain.Interfaces
{
    public interface IUserService
    {
        Task<Users> CreateUserAsync(CreateUserDTO dto);
        Task<Users> GetUserAsync(string username);
        Task<List<Users>> GetAllUsersAsync();
        Task<List<Users>> GetUsersByGroupAsync(int groupId);
        Task<bool> UpdateUserAsync(string username, UpdateUserDTO dto);
        Task<bool> DeleteUserAsync(string username);
        Task<bool> UserExistsAsync(string username);
    }
}