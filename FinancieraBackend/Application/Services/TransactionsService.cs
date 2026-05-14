using FinancieraBackend.Domain.Interfaces;
using FinancieraBackend.Domain.Models;
using FinancieraBackend.Domain.DTOs;
using FinancieraBackend.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FinancieraBackend.Application.Services
{
    public class TransactionsService : ITransactionsService
    {
        private readonly DBConnection _context;

        public TransactionsService(DBConnection context)
        {
            _context = context;
        }

        public async Task<Transactions> CreateTransactionAsync(CreateTransactionDTO dto)
        {
            var transaction = new Transactions
            {
                GroupId = dto.GroupId,
                UserId = dto.UserId,
                Amount = dto.Amount,
                Type = dto.Type,
                Status = TransactionStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return transaction;
        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return false;

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

            transaction.Amount = dto.Amount;
            transaction.Type = dto.Type;

            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
