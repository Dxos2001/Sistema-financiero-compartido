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
    public class FinancialGroupsController : ControllerBase
    {
        private readonly IFinancialGroupService _groupService;
        private readonly IGroupMemberService _memberService;

        public FinancialGroupsController(
            IFinancialGroupService groupService,
            IGroupMemberService memberService)
        {
            _groupService = groupService;
            _memberService = memberService;
        }

        [HttpPost]
        public async Task<ActionResult<FinancialGroups>> Create([FromBody] CreateFinancialGroupDTO dto)
        {
            var result = await _groupService.CreateGroupAsync(dto);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FinancialGroups>> Get(int id)
        {
            var result = await _groupService.GetGroupAsync(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<FinancialGroups>>> GetAll()
        {
            var result = await _groupService.GetAllGroupsAsync();
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Update(int id, [FromBody] UpdateFinancialGroupDTO dto)
        {
            var userId = User.GetUserId();
            var role = await _memberService.GetUserRoleInGroupAsync(id, userId);

            if (role != Role.Admin)
                return Forbid();

            var result = await _groupService.UpdateGroupAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var userId = User.GetUserId();
            var role = await _memberService.GetUserRoleInGroupAsync(id, userId);

            if (role != Role.Admin)
                return Forbid();

            var result = await _groupService.DeleteGroupAsync(id);
            return Ok(result);
        }
    }
}
