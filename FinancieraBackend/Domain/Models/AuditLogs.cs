using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancieraBackend.Domain.Models
{
    [Table("AuditLogs")]
    public class AuditLogs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Transaction")]
        public int TransactionId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Action { get; set; }

        [Required]
        [MaxLength(256)]
        public string HashIntegrity { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        // Navigation
        public Transactions Transaction { get; set; }
    }
}