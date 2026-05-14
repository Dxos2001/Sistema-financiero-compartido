using FinancieraBackend.Domain.Interfaces;
using FinancieraBackend.Domain.Models;
using FinancieraBackend.Domain.DTOs;
using FinancieraBackend.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FinancieraBackend.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly DBConnection _context;

        public AuthService(DBConnection context)
        {
            _context = context;
        }

        public async Task<string> AuthenticateAsync(LoginDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            
            if (user == null)
            {
                return null;
            }

            // AQUI IRIA LA VERIFICACION DEL PASSWORD HASH, EJ: BCrypt.Net.BCrypt.Verify(dto.Password, user.Password)
            // Ya que el modelo de User no incluye propiedad "Password" en la DB segun el modelo actual:
            // return "fake-jwt-token-for-development";
            
            // Retorno un string simulado ya que el modelo Users no tiene la columna Password
            return "simulated_jwt_token_for_" + user.Username;
        }
    }
}
