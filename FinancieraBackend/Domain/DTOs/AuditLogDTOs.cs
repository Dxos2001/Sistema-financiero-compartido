using System.ComponentModel.DataAnnotations;

namespace FinancieraBackend.Domain.DTOs
{
    public class CreateAuditLogDTO
    {
        [Required]
        public int TransactionId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Action { get; set; }

        [Required]
        [MaxLength(256)]
        public string HashIntegrity { get; set; }
    }
}
