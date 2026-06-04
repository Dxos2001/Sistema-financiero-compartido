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
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionsService _transactionsService;
        private readonly IGroupMemberService _memberService;

        public TransactionsController(
            ITransactionsService transactionsService,
            IGroupMemberService memberService)
        {
            _transactionsService = transactionsService;
            _memberService = memberService;
        }

        [HttpPost]
        public async Task<ActionResult<Transactions>> Create([FromBody] CreateTransactionDTO dto)
        {
            var userId = User.GetUserId();
            var role = await _memberService.GetUserRoleInGroupAsync(dto.GroupId, userId);

            if (role == null)
                return Forbid(); // Solo miembros pueden crear transacciones

            var result = await _transactionsService.CreateTransactionAsync(dto, role.Value);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Transactions>> Get(int id)
        {
            var result = await _transactionsService.GetTransactionAsync(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<Transactions>>> GetAll()
        {
            var result = await _transactionsService.GetAllTransactionsAsync();
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Update(int id, [FromBody] UpdateTransactionDTO dto)
        {
            var transaction = await _transactionsService.GetTransactionAsync(id);
            if (transaction == null) return NotFound();

            var userId = User.GetUserId();
            var role = await _memberService.GetUserRoleInGroupAsync(transaction.GroupId, userId);

            if (role != Role.Admin)
                return Forbid(); // Solo admins pueden editar (y aprobar/rechazar)

            var result = await _transactionsService.UpdateTransactionAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var transaction = await _transactionsService.GetTransactionAsync(id);
            if (transaction == null) return NotFound();

            var userId = User.GetUserId();
            var role = await _memberService.GetUserRoleInGroupAsync(transaction.GroupId, userId);

            if (role != Role.Admin)
                return Forbid();

            var result = await _transactionsService.DeleteTransactionAsync(id);
            return Ok(result);
        }
    }
}
