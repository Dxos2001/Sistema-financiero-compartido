using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancieraBackend.Domain.Models
{
    [Table("Users")]
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public int PersonId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("PersonId")]
        public Persons Person { get; set; }

        public ICollection<FinancialGroups> CreatedGroups { get; set; }
        public ICollection<GroupMembers> GroupMemberships { get; set; }
        public ICollection<Transactions> Transactions { get; set; }
        public ICollection<ConsensusRequests> RequestedConsensus { get; set; }
    }
}