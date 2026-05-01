using FinancieraBackend.Domain.Interfaces;
using FinancieraBackend.Domain.Models;
using FinancieraBackend.Domain.DTOs;
using FinancieraBackend.Infrastructure;

namespace FinancieraBackend.Application.Services
{
    public class GroupMemberService : IGroupMemberService
    {
        private readonly DBConnection _context;

        public GroupMemberService(DBConnection context)
        {
            _context = context;
        }

        public Task<GroupMembers> AddMemberAsync(AddGroupMemberDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<List<GroupMembers>> GetGroupsByUserAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<GroupMembers>> GetMembersByGroupAsync(int groupId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsMemberAsync(int groupId, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveMemberAsync(int groupId, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateMemberRoleAsync(int groupId, int userId, UpdateGroupMemberRoleDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
