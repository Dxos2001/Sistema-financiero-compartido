using FinancieraBackend.Domain.Models;
using FinancieraBackend.Domain.DTOs;

namespace FinancieraBackend.Domain.Interfaces
{
    public interface IGroupMemberService
    {
        Task<GroupMembers> AddMemberAsync(AddGroupMemberDTO dto);
        Task<bool> RemoveMemberAsync(int groupId, int userId);
        Task<bool> UpdateMemberRoleAsync(int groupId, int userId, UpdateGroupMemberRoleDTO dto);
        Task<List<GroupMembers>> GetMembersByGroupAsync(int groupId);
        Task<List<GroupMembers>> GetGroupsByUserAsync(int userId);
        Task<bool> IsMemberAsync(int groupId, int userId);
        Task<Role?> GetUserRoleInGroupAsync(int groupId, int userId);
    }
}
