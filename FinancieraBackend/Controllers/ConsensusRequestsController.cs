using Microsoft.AspNetCore.Mvc;
using FinancieraBackend.Domain.Interfaces;
using FinancieraBackend.Domain.DTOs;
using FinancieraBackend.Domain.Models;

namespace FinancieraBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsensusRequestsController : ControllerBase
    {
        private readonly IConsensusRequestService _consensusService;

        public ConsensusRequestsController(IConsensusRequestService consensusService)
        {
            _consensusService = consensusService;
        }

        [HttpPost]
        public async Task<ActionResult<ConsensusRequests>> Create([FromBody] CreateConsensusRequestDTO dto)
        {
            var result = await _consensusService.CreateRequestAsync(dto);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ConsensusRequests>> Get(int id)
        {
            var result = await _consensusService.GetRequestAsync(id);
            return Ok(result);
        }

        [HttpGet("transaction/{transactionId}")]
        public async Task<ActionResult<List<ConsensusRequests>>> GetByTransaction(int transactionId)
        {
            var result = await _consensusService.GetRequestsByTransactionAsync(transactionId);
            return Ok(result);
        }

        [HttpGet("pending/{approverId}")]
        public async Task<ActionResult<List<ConsensusRequests>>> GetPendingForApprover(int approverId)
        {
            var result = await _consensusService.GetPendingRequestsForApproverAsync(approverId);
            return Ok(result);
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult<bool>> UpdateStatus(int id, [FromBody] UpdateConsensusRequestStatusDTO dto)
        {
            var result = await _consensusService.UpdateRequestStatusAsync(id, dto);
            return Ok(result);
        }
    }
}
