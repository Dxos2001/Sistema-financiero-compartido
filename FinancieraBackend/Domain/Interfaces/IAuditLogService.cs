using FinancieraBackend.Domain.Models;
using FinancieraBackend.Domain.DTOs;

namespace FinancieraBackend.Domain.Interfaces
{
    public interface IAuditLogService
    {
        Task<AuditLogs> CreateLogAsync(CreateAuditLogDTO dto);
        Task<AuditLogs> GetLogAsync(int id);
        Task<List<AuditLogs>> GetLogsByTransactionAsync(int transactionId);
        Task<List<AuditLogs>> GetAllLogsAsync();
    }
}
