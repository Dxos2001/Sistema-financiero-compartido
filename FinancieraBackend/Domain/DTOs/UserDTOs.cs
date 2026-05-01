using System.ComponentModel.DataAnnotations;

namespace FinancieraBackend.Domain.DTOs
{
    public class CreateUserDTO
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public int PersonId { get; set; }
    }

    public class UpdateUserDTO
    {
        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        public string? Password { get; set; }
    }
}
