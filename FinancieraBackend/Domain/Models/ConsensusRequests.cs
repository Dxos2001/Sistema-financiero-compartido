using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancieraBackend.Domain.Models
{
    [Table("ConsensusRequests")]
    public class ConsensusRequests
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Transaction")]
        public int TransactionId { get; set; }

        [ForeignKey("RequestedByUser")]
        public int RequestedBy { get; set; }

        public ConsensusAction Action { get; set; }

        [ForeignKey("Approver")]
        public int? ApproverId { get; set; }

        public ConsensusStatus Status { get; set; } = ConsensusStatus.Pending;

        public DateTime RequestedAt { get; set; } = DateTime.Now;

        public DateTime? ResolvedAt { get; set; }

        // Navigation
        public Transactions Transaction { get; set; }
        public Users RequestedByUser { get; set; }
    }

    public enum ConsensusAction
    {
        Create,
        Update,
        Delete
    }

    public enum ConsensusStatus
    {
        Pending,
        Approved,
        Rejected
    }
}