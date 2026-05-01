using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancieraBackend.Domain.Models
{
    [Table("Transactions")]
    public class Transactions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Group")]
        public int GroupId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal Amount { get; set; }

        public TransactionType Type { get; set; }

        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public FinancialGroups Group { get; set; }
        public Users User { get; set; }
        public ICollection<ConsensusRequests> ConsensusRequests { get; set; }
        public ICollection<AuditLogs> AuditLogs { get; set; }
    }

    public enum TransactionType
    {
        Income,
        Expense
    }

    public enum TransactionStatus
    {
        Pending,
        Approved,
        Rejected
    }
}