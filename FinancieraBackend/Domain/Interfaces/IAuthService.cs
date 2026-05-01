using FinancieraBackend.Domain.Models;
using FinancieraBackend.Domain.DTOs;

namespace FinancieraBackend.Domain.Interfaces
{
    public interface IAuthService
    {
        Task<string> AuthenticateAsync(LoginDTO dto);
    }
}