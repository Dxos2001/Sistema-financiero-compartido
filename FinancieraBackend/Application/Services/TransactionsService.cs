using FinancieraBackend.Domain.Interfaces;
using FinancieraBackend.Domain.Models;
using FinancieraBackend.Domain.DTOs;
using FinancieraBackend.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
namespace FinancieraBackend.Application.Services
{
    public class TransactionsService : ITransactionsService
    {
        private readonly DBConnection _context;

        public TransactionsService(DBConnection context)
        {
            _context = context;
        }

        public async Task<Transactions> CreateTransactionAsync(CreateTransactionDTO dto, Role creatorRole)
        {
            var transaction = new Transactions
            {
                GroupId = dto.GroupId,
                UserId = dto.UserId,
                Amount = dto.Amount,
                Type = dto.Type,
                Status = creatorRole == Role.Admin ? TransactionStatus.Approved : TransactionStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            // Auditoría automática
            var auditLog = new AuditLogs
            {
                TransactionId = transaction.Id,
                Action = "Create",
                HashIntegrity = GenerateHash(transaction),
                Timestamp = DateTime.UtcNow
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            // Update FinancialGroup balance
            var group = await _context.FinancialGroups.FindAsync(transaction.GroupId);
            if (group != null)
            {
                if (transaction.Type == TransactionType.Income)
                    group.Balance += transaction.Amount;
                else
                    group.Balance -= transaction.Amount;

                _context.FinancialGroups.Update(group);
                await _context.SaveChangesAsync();
            }

            return transaction;
        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return false;

            // Auditoría automática
            var auditLog = new AuditLogs
            {
                TransactionId = transaction.Id,
                Action = "Delete",
                HashIntegrity = GenerateHash(transaction),
                Timestamp = DateTime.UtcNow
            };
            _context.AuditLogs.Add(auditLog);

            // Revert FinancialGroup balance
            var group = await _context.FinancialGroups.FindAsync(transaction.GroupId);
            if (group != null)
            {
                if (transaction.Type == TransactionType.Income)
                    group.Balance -= transaction.Amount;
                else
                    group.Balance += transaction.Amount;

                _context.FinancialGroups.Update(group);
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Transactions>> GetAllTransactionsAsync()
        {
            return await _context.Transactions.ToListAsync();
        }

        public async Task<Transactions> GetTransactionAsync(int id)
        {
            return await _context.Transactions.FindAsync(id);
        }

        public async Task<bool> TransactionExistsAsync(int id)
        {
            return await _context.Transactions.AnyAsync(t => t.Id == id);
        }

        public async Task<bool> UpdateTransactionAsync(int id, UpdateTransactionDTO dto)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return false;

            // Adjust FinancialGroup balance
            var group = await _context.FinancialGroups.FindAsync(transaction.GroupId);
            if (group != null)
            {
                // Revert old transaction impact
                if (transaction.Type == TransactionType.Income)
                    group.Balance -= transaction.Amount;
                else
                    group.Balance += transaction.Amount;

                // Apply new transaction impact
                if (dto.Type == TransactionType.Income)
                    group.Balance += dto.Amount;
                else
                    group.Balance -= dto.Amount;

                _context.FinancialGroups.Update(group);
            }

            transaction.Amount = dto.Amount;
            transaction.Type = dto.Type;

            _context.Transactions.Update(transaction);

            // Auditoría automática
            var auditLog = new AuditLogs
            {
                TransactionId = transaction.Id,
                Action = "Update",
                HashIntegrity = GenerateHash(transaction),
                Timestamp = DateTime.UtcNow
            };
            _context.AuditLogs.Add(auditLog);

            await _context.SaveChangesAsync();
            return true;
        }

        private string GenerateHash(Transactions t)
        {
            using (var sha256 = SHA256.Create())
            {
                var rawData = $"{t.Id}-{t.Amount}-{t.Type}-{t.Status}-{t.CreatedAt:O}";
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
