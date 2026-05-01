using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancieraBackend.Domain.Models
{
    [Table("GroupMembers")]
    public class GroupMembers
    {
        [ForeignKey("Group")]
        public int GroupId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public Role Role { get; set; } = Role.Member;

        public FinancialGroups Group { get; set; }
        public Users User { get; set; }
    }

    public enum Role
    {
        Admin,
        Member
    }
}