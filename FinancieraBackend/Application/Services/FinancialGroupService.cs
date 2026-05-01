using FinancieraBackend.Domain.Interfaces;
using FinancieraBackend.Domain.Models;
using FinancieraBackend.Domain.DTOs;
using FinancieraBackend.Infrastructure;

namespace FinancieraBackend.Application.Services
{
    public class FinancialGroupService : IFinancialGroupService
    {
        private readonly DBConnection _context;

        public FinancialGroupService(DBConnection context)
        {
            _context = context;
        }

        public Task<FinancialGroups> CreateGroupAsync(CreateFinancialGroupDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteGroupAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<FinancialGroups>> GetAllGroupsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<FinancialGroups> GetGroupAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GroupExistsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateGroupAsync(int id, UpdateFinancialGroupDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
