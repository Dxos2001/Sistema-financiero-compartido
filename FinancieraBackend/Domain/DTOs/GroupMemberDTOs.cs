using System.ComponentModel.DataAnnotations;
using FinancieraBackend.Domain.Models;

namespace FinancieraBackend.Domain.DTOs
{
    public class AddGroupMemberDTO
    {
        [Required]
        public int GroupId { get; set; }

        [Required]
        public int UserId { get; set; }

        public Role Role { get; set; } = Role.Member;
    }

    public class UpdateGroupMemberRoleDTO
    {
        [Required]
        public Role Role { get; set; }
    }
}
