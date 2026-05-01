using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FinancieraBackend.Domain.Models;

namespace FinancieraBackend.Domain.DTOs
{
    public class CreateTransactionDTO
    {
        [Required]
        public int GroupId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public TransactionType Type { get; set; }
    }

    public class UpdateTransactionDTO
    {
        [Required]
        public decimal Amount { get; set; }

        [Required]
        public TransactionType Type { get; set; }
    }
}
