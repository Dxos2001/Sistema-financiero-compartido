using FinancieraBackend.Domain.Models;
using FinancieraBackend.Domain.DTOs;

namespace FinancieraBackend.Domain.Interfaces
{
    public interface IPersonService
    {
        Task<Persons> CreatePersonAsync(CreatePersonDTO dto);
        Task<Persons> GetPersonAsync(string identityNumber);
        Task<List<Persons>> GetAllPersonsAsync();
        Task<bool> UpdatePersonAsync(string identityNumber, UpdatePersonDTO dto);
        Task<bool> DeletePersonAsync(string identityNumber);
        Task<bool> PersonExistsAsync(string identityNumber);
    }
}