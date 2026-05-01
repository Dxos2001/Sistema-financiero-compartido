using FinancieraBackend.Domain.Interfaces;
using FinancieraBackend.Domain.Models;
using FinancieraBackend.Domain.DTOs;
using FinancieraBackend.Infrastructure;

namespace FinancieraBackend.Application.Services
{
    public class TransactionsService : ITransactionsService
    {
        private readonly DBConnection _context;

        public TransactionsService(DBConnection context)
        {
            _context = context;
        }

        public Task<Transactions> CreateTransactionAsync(CreateTransactionDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteTransactionAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Transactions>> GetAllTransactionsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Transactions> GetTransactionAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> TransactionExistsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateTransactionAsync(int id, UpdateTransactionDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
