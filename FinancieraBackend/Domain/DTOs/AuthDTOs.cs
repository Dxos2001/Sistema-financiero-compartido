using System.ComponentModel.DataAnnotations;

namespace FinancieraBackend.Domain.DTOs
{
    // ──────────────────────────────────────────────────────────────────────────
    // Login
    // ──────────────────────────────────────────────────────────────────────────

    public class LoginDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Registro unificado — crea Person + User en una sola llamada
    // NOTE: Cuando UseAwsIam=true, el campo Password NO se almacenará.
    //       La autenticación se delegará a AWS IAM usando el secret "IAMSecret".
    // ──────────────────────────────────────────────────────────────────────────

    public class RegisterDTO
    {
        // ── Credenciales de acceso ────────────────────────────────────────────
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Contraseña en texto plano. El servicio la hashea con SHA-256 antes
        /// de persistir. Ignorada (y no persistida) cuando UseAwsIam = true.
        /// </summary>
        [Required]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        public string Password { get; set; }

        // ── Datos personales (se crea Persons en la misma transacción) ────────
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [MaxLength(50)]
        public string? MiddleName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(20)]
        public string? IdentificationNumber { get; set; }

        [MaxLength(15)]
        public string? Phone { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Respuesta de autenticación (login y register)
    // ──────────────────────────────────────────────────────────────────────────

    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
