using FinancieraBackend.Domain.Interfaces;
using FinancieraBackend.Domain.DTOs;
using FinancieraBackend.Infrastructure;

namespace FinancieraBackend.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly DBConnection _context;

        public AuthService(DBConnection context)
        {
            _context = context;
        }

        public Task<string> AuthenticateAsync(LoginDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
