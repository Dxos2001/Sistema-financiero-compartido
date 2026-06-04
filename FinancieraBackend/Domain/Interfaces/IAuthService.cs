using FinancieraBackend.Domain.DTOs;

namespace FinancieraBackend.Domain.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Autentica un usuario con username y contraseña.
        /// Cuando UseAwsIam = true, delega la validación a AWS IAM (IAMSecret).
        /// Cuando UseAwsIam = false, verifica el hash SHA-256 local.
        /// </summary>
        Task<AuthResponseDTO?> AuthenticateAsync(LoginDTO dto);

        /// <summary>
        /// Registra un nuevo usuario creando Person + User en una transacción atómica.
        /// Cuando UseAwsIam = true, no persiste PasswordHash.
        /// Cuando UseAwsIam = false, hashea la contraseña con SHA-256 y la persiste.
        /// </summary>
        Task<AuthResponseDTO> RegisterAsync(RegisterDTO dto);
    }
}