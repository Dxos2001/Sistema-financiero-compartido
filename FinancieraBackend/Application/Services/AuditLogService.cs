using FinancieraBackend.Domain.Interfaces;
using FinancieraBackend.Domain.Models;
using FinancieraBackend.Domain.DTOs;
using FinancieraBackend.Infrastructure;

namespace FinancieraBackend.Application.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly DBConnection _context;

        public AuditLogService(DBConnection context)
        {
            _context = context;
        }

        public Task<AuditLogs> CreateLogAsync(CreateAuditLogDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<List<AuditLogs>> GetAllLogsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AuditLogs> GetLogAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<AuditLogs>> GetLogsByTransactionAsync(int transactionId)
        {
            throw new NotImplementedException();
        }
    }
}
