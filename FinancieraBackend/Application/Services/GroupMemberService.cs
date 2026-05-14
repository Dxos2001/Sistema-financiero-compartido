using FinancieraBackend.Domain.Interfaces;
using FinancieraBackend.Domain.Models;
using FinancieraBackend.Domain.DTOs;
using FinancieraBackend.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FinancieraBackend.Application.Services
{
    public class GroupMemberService : IGroupMemberService
    {
        private readonly DBConnection _context;

        public GroupMemberService(DBConnection context)
        {
            _context = context;
        }

        public async Task<GroupMembers> AddMemberAsync(AddGroupMemberDTO dto)
        {
            var member = new GroupMembers
            {
                GroupId = dto.GroupId,
                UserId = dto.UserId,
                Role = dto.Role
            };

            _context.GroupMembers.Add(member);
            await _context.SaveChangesAsync();

            return member;
        }

        public async Task<List<GroupMembers>> GetGroupsByUserAsync(int userId)
        {
            return await _context.GroupMembers
                .Where(gm => gm.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<GroupMembers>> GetMembersByGroupAsync(int groupId)
        {
            return await _context.GroupMembers
                .Where(gm => gm.GroupId == groupId)
                .ToListAsync();
        }

        public async Task<bool> IsMemberAsync(int groupId, int userId)
        {
            return await _context.GroupMembers
                .AnyAsync(gm => gm.GroupId == groupId && gm.UserId == userId);
        }

        public async Task<bool> RemoveMemberAsync(int groupId, int userId)
        {
            var member = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

            if (member == null) return false;

            _context.GroupMembers.Remove(member);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateMemberRoleAsync(int groupId, int userId, UpdateGroupMemberRoleDTO dto)
        {
            var member = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

            if (member == null) return false;

            member.Role = dto.Role;

            _context.GroupMembers.Update(member);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
