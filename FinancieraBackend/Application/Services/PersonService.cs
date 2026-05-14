using FinancieraBackend.Domain.Interfaces;
using FinancieraBackend.Domain.Models;
using FinancieraBackend.Domain.DTOs;
using FinancieraBackend.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FinancieraBackend.Application.Services
{
    public class PersonService : IPersonService
    {
        private readonly DBConnection _context;

        public PersonService(DBConnection context)
        {
            _context = context;
        }

        public async Task<Persons> CreatePersonAsync(CreatePersonDTO dto)
        {
            var person = new Persons
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                MiddleName = dto.MiddleName,
                DateOfBirth = dto.DateOfBirth,
                IdentificationNumber = dto.IdentificationNumber,
                Phone = dto.Phone,
                Address = dto.Address,
                CreatedAt = DateTime.UtcNow
            };

            _context.Persons.Add(person);
            await _context.SaveChangesAsync();

            return person;
        }

        public async Task<bool> DeletePersonAsync(string identityNumber)
        {
            var person = await _context.Persons.FirstOrDefaultAsync(p => p.IdentificationNumber == identityNumber);
            if (person == null) return false;

            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Persons>> GetAllPersonsAsync()
        {
            return await _context.Persons.ToListAsync();
        }

        public async Task<Persons> GetPersonAsync(string identityNumber)
        {
            return await _context.Persons.FirstOrDefaultAsync(p => p.IdentificationNumber == identityNumber);
        }

        public async Task<bool> PersonExistsAsync(string identityNumber)
        {
            return await _context.Persons.AnyAsync(p => p.IdentificationNumber == identityNumber);
        }

        public async Task<bool> UpdatePersonAsync(string identityNumber, UpdatePersonDTO dto)
        {
            var person = await _context.Persons.FirstOrDefaultAsync(p => p.IdentificationNumber == identityNumber);
            if (person == null) return false;

            person.FirstName = dto.FirstName;
            person.LastName = dto.LastName;
            person.MiddleName = dto.MiddleName;
            person.DateOfBirth = dto.DateOfBirth;
            person.IdentificationNumber = dto.IdentificationNumber;
            person.Phone = dto.Phone;
            person.Address = dto.Address;

            _context.Persons.Update(person);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
