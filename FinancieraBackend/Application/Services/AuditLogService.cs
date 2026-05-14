using FinancieraBackend.Domain.Interfaces;
using FinancieraBackend.Domain.Models;
using FinancieraBackend.Domain.DTOs;
using FinancieraBackend.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FinancieraBackend.Application.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly DBConnection _context;

        public AuditLogService(DBConnection context)
        {
            _context = context;
        }

        public async Task<AuditLogs> CreateLogAsync(CreateAuditLogDTO dto)
        {
            var auditLog = new AuditLogs
            {
                TransactionId = dto.TransactionId,
                Action = dto.Action,
                HashIntegrity = dto.HashIntegrity,
                Timestamp = DateTime.UtcNow
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            return auditLog;
        }

        public async Task<List<AuditLogs>> GetAllLogsAsync()
        {
            return await _context.AuditLogs.ToListAsync();
        }

        public async Task<AuditLogs> GetLogAsync(int id)
        {
            return await _context.AuditLogs.FindAsync(id);
        }

        public async Task<List<AuditLogs>> GetLogsByTransactionAsync(int transactionId)
        {
            return await _context.AuditLogs
                .Where(a => a.TransactionId == transactionId)
                .ToListAsync();
        }
    }
}
