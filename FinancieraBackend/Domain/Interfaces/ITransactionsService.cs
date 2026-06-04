using FinancieraBackend.Domain.Models;
using FinancieraBackend.Domain.DTOs;

namespace FinancieraBackend.Domain.Interfaces
{
    public interface ITransactionsService
    {
        Task<Transactions> CreateTransactionAsync(CreateTransactionDTO dto, Role creatorRole);
        Task<Transactions> GetTransactionAsync(int id);
        Task<List<Transactions>> GetAllTransactionsAsync();
        Task<bool> UpdateTransactionAsync(int id, UpdateTransactionDTO dto);
        Task<bool> DeleteTransactionAsync(int id);
        Task<bool> TransactionExistsAsync(int id);
    }
}