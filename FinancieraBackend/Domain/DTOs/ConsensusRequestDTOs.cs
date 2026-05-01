using System.ComponentModel.DataAnnotations;
using FinancieraBackend.Domain.Models;

namespace FinancieraBackend.Domain.DTOs
{
    public class CreateConsensusRequestDTO
    {
        [Required]
        public int TransactionId { get; set; }

        [Required]
        public int RequestedBy { get; set; }

        [Required]
        public ConsensusAction Action { get; set; }

        public int? ApproverId { get; set; }
    }

    public class UpdateConsensusRequestStatusDTO
    {
        [Required]
        public ConsensusStatus Status { get; set; }
        
        [Required]
        public int ResolvedByUserId { get; set; }
    }
}
