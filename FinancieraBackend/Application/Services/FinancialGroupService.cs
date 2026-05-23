using FinancieraBackend.Domain.Interfaces;
using FinancieraBackend.Domain.Models;
using FinancieraBackend.Domain.DTOs;
using FinancieraBackend.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FinancieraBackend.Application.Services
{
    public class FinancialGroupService : IFinancialGroupService
    {
        private readonly DBConnection _context;

        public FinancialGroupService(DBConnection context)
        {
            _context = context;
        }

        public async Task<FinancialGroups> CreateGroupAsync(CreateFinancialGroupDTO dto)
        {
            var group = new FinancialGroups
            {
                Name = dto.Name,
                CreatedBy = dto.CreatedBy,
                Balance = 0.00m,
                CreatedAt = DateTime.UtcNow
            };

            _context.FinancialGroups.Add(group);
            await _context.SaveChangesAsync();

            return group;
        }

        public async Task<bool> DeleteGroupAsync(int id)
        {
            var group = await _context.FinancialGroups.FindAsync(id);
            if (group == null) return false;

            _context.FinancialGroups.Remove(group);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<FinancialGroups>> GetAllGroupsAsync()
        {
            var groups = await _context.FinancialGroups.ToListAsync();

            var balances = await _context.Transactions
                .GroupBy(t => t.GroupId)
                .Select(g => new
                {
                    GroupId = g.Key,
                    Income = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                    Expense = g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)
                })
                .ToDictionaryAsync(x => x.GroupId, x => x.Income - x.Expense);

            foreach (var g in groups)
            {
                if (balances.TryGetValue(g.Id, out decimal balance))
                    g.Balance = balance;
                else
                    g.Balance = 0;
            }

            return groups;
        }

        public async Task<FinancialGroups> GetGroupAsync(int id)
        {
            var group = await _context.FinancialGroups.FindAsync(id);
            if (group != null)
            {
                var income = await _context.Transactions.Where(t => t.GroupId == id && t.Type == TransactionType.Income).SumAsync(t => t.Amount);
                var expense = await _context.Transactions.Where(t => t.GroupId == id && t.Type == TransactionType.Expense).SumAsync(t => t.Amount);
                group.Balance = income - expense;
            }
            return group;
        }

        public async Task<bool> GroupExistsAsync(int id)
        {
            return await _context.FinancialGroups.AnyAsync(g => g.Id == id);
        }

        public async Task<bool> UpdateGroupAsync(int id, UpdateFinancialGroupDTO dto)
        {
            var group = await _context.FinancialGroups.FindAsync(id);
            if (group == null) return false;

            group.Name = dto.Name;

            _context.FinancialGroups.Update(group);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
