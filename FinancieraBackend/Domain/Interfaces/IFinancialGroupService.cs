using FinancieraBackend.Domain.Models;
using FinancieraBackend.Domain.DTOs;

namespace FinancieraBackend.Domain.Interfaces
{
    public interface IFinancialGroupService
    {
        Task<FinancialGroups> CreateGroupAsync(CreateFinancialGroupDTO dto);
        Task<FinancialGroups> GetGroupAsync(int id);
        Task<List<FinancialGroups>> GetAllGroupsAsync();
        Task<bool> UpdateGroupAsync(int id, UpdateFinancialGroupDTO dto);
        Task<bool> DeleteGroupAsync(int id);
        Task<bool> GroupExistsAsync(int id);
    }
}
