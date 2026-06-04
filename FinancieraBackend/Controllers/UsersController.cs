using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinancieraBackend.Domain.Interfaces;
using FinancieraBackend.Domain.DTOs;
using FinancieraBackend.Domain.Models;

namespace FinancieraBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<Users>> Create([FromBody] CreateUserDTO dto)
        {
            var result = await _userService.CreateUserAsync(dto);
            return Ok(result);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<Users>> Get(string username)
        {
            var result = await _userService.GetUserAsync(username);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<Users>>> GetAll()
        {
            var result = await _userService.GetAllUsersAsync();
            return Ok(result);
        }

        /// <summary>
        /// Returns all users that are members of the given group.
        /// Used by the frontend to populate the User combobox after a group is selected.
        /// </summary>
        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<List<Users>>> GetByGroup(int groupId)
        {
            var result = await _userService.GetUsersByGroupAsync(groupId);
            return Ok(result);
        }

        [HttpPut("{username}")]
        public async Task<ActionResult<bool>> Update(string username, [FromBody] UpdateUserDTO dto)
        {
            var result = await _userService.UpdateUserAsync(username, dto);
            return Ok(result);
        }

        [HttpDelete("{username}")]
        public async Task<ActionResult<bool>> Delete(string username)
        {
            var result = await _userService.DeleteUserAsync(username);
            return Ok(result);
        }
    }
}
