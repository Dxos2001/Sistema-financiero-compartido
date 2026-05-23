using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancieraBackend.Domain.Models
{
    [Table("FinancialGroups")]
    public class FinancialGroups
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [ForeignKey("Creator")]
        public int CreatedBy { get; set; }

        public Users Creator { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal Balance { get; set; } = 0.00m;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column(TypeName = "longtext")]
        public string? CachedProjection { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? LastProjectionBalance { get; set; }

        // Navigation
        public ICollection<GroupMembers> Members { get; set; }
        public ICollection<Transactions> Transactions { get; set; }
    }
}