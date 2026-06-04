using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinancieraBackend.Domain.Interfaces;
using FinancieraBackend.Domain.DTOs;
using FinancieraBackend.Domain.Models;
using FinancieraBackend.Extensions;

namespace FinancieraBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class GroupMembersController : ControllerBase
    {
        private readonly IGroupMemberService _memberService;

        public GroupMembersController(IGroupMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpPost]
        public async Task<ActionResult<GroupMembers>> Add([FromBody] AddGroupMemberDTO dto)
        {
            var currentUserId = User.GetUserId();
            var currentUserRole = await _memberService.GetUserRoleInGroupAsync(dto.GroupId, currentUserId);

            if (currentUserRole != Role.Admin)
                return Forbid();

            var result = await _memberService.AddMemberAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{groupId}/{userId}")]
        public async Task<ActionResult<bool>> Remove(int groupId, int userId)
        {
            var currentUserId = User.GetUserId();
            var currentUserRole = await _memberService.GetUserRoleInGroupAsync(groupId, currentUserId);

            // Un Admin puede eliminar a otros. Un usuario también puede eliminarse a sí mismo (salir del grupo).
            if (currentUserRole != Role.Admin && currentUserId != userId)
                return Forbid();

            var result = await _memberService.RemoveMemberAsync(groupId, userId);
            return Ok(result);
        }

        [HttpPut("{groupId}/{userId}/role")]
        public async Task<ActionResult<bool>> UpdateRole(int groupId, int userId, [FromBody] UpdateGroupMemberRoleDTO dto)
        {
            var currentUserId = User.GetUserId();
            var currentUserRole = await _memberService.GetUserRoleInGroupAsync(groupId, currentUserId);

            if (currentUserRole != Role.Admin)
                return Forbid();

            var result = await _memberService.UpdateMemberRoleAsync(groupId, userId, dto);
            return Ok(result);
        }

        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<List<GroupMembers>>> GetByGroup(int groupId)
        {
            var result = await _memberService.GetMembersByGroupAsync(groupId);
            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<GroupMembers>>> GetByUser(int userId)
        {
            var result = await _memberService.GetGroupsByUserAsync(userId);
            return Ok(result);
        }
    }
}
