using Microsoft.AspNetCore.Mvc;
using FinancieraBackend.Domain.Interfaces;
using FinancieraBackend.Domain.DTOs;
using FinancieraBackend.Domain.Models;

namespace FinancieraBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionsService _transactionsService;

        public TransactionsController(ITransactionsService transactionsService)
        {
            _transactionsService = transactionsService;
        }

        [HttpPost]
        public async Task<ActionResult<Transactions>> Create([FromBody] CreateTransactionDTO dto)
        {
            var result = await _transactionsService.CreateTransactionAsync(dto);
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
            var result = await _transactionsService.UpdateTransactionAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var result = await _transactionsService.DeleteTransactionAsync(id);
            return Ok(result);
        }
    }
}
