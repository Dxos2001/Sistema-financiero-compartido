using FinancieraBackend.Domain.Interfaces;
using FinancieraBackend.Domain.Models;
using FinancieraBackend.Domain.DTOs;
using FinancieraBackend.Infrastructure;

namespace FinancieraBackend.Application.Services
{
    public class PersonService : IPersonService
    {
        private readonly DBConnection _context;

        public PersonService(DBConnection context)
        {
            _context = context;
        }

        public Task<Persons> CreatePersonAsync(CreatePersonDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeletePersonAsync(string identityNumber)
        {
            throw new NotImplementedException();
        }

        public Task<List<Persons>> GetAllPersonsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Persons> GetPersonAsync(string identityNumber)
        {
            throw new NotImplementedException();
        }

        public Task<bool> PersonExistsAsync(string identityNumber)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdatePersonAsync(string identityNumber, UpdatePersonDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
