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
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogsController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        [HttpPost]
        public async Task<ActionResult<AuditLogs>> Create([FromBody] CreateAuditLogDTO dto)
        {
            var result = await _auditLogService.CreateLogAsync(dto);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuditLogs>> Get(int id)
        {
            var result = await _auditLogService.GetLogAsync(id);
            return Ok(result);
        }

        [HttpGet("transaction/{transactionId}")]
        public async Task<ActionResult<List<AuditLogs>>> GetByTransaction(int transactionId)
        {
            var result = await _auditLogService.GetLogsByTransactionAsync(transactionId);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<AuditLogs>>> GetAll()
        {
            var result = await _auditLogService.GetAllLogsAsync();
            return Ok(result);
        }
    }
}
