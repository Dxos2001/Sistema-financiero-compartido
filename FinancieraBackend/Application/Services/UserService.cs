using FinancieraBackend.Domain.Interfaces;
using FinancieraBackend.Domain.Models;
using FinancieraBackend.Domain.DTOs;
using FinancieraBackend.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FinancieraBackend.Application.Services
{
    public class UserService : IUserService
    {
        private readonly DBConnection _context;

        public UserService(DBConnection context)
        {
            _context = context;
        }

        public async Task<Users> CreateUserAsync(CreateUserDTO dto)
        {
            // Note: El modelo Users actual no tiene propiedad Password mapeada, 
            // asumimos que será agregada o manejada luego.
            var user = new Users
            {
                Username = dto.Username,
                Email = dto.Email,
                PersonId = dto.PersonId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<bool> DeleteUserAsync(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Users>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<Users> GetUserAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> UpdateUserAsync(string username, UpdateUserDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return false;

            user.Email = dto.Email;
            
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<List<Users>> GetUsersByGroupAsync(int groupId)
        {
            return await _context.GroupMembers
                .Where(gm => gm.GroupId == groupId)
                .Include(gm => gm.User)
                    .ThenInclude(u => u.Person)
                .Select(gm => gm.User)
                .ToListAsync();
        }
    }
}
