using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FinancieraBackend.Domain.DTOs;
using FinancieraBackend.Domain.Interfaces;
using FinancieraBackend.Domain.Models;
using FinancieraBackend.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FinancieraBackend.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly DBConnection _context;
        private readonly IConfiguration _configuration;

        public AuthService(DBConnection context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ──────────────────────────────────────────────────────────────────────
        // Login
        // ──────────────────────────────────────────────────────────────────────

        public async Task<AuthResponseDTO?> AuthenticateAsync(LoginDTO dto)
        {
            var useAwsIam = _configuration.GetValue<bool>("UseAwsIam");

            if (useAwsIam)
            {
                // ── STUB AWS IAM ───────────────────────────────────────────────
                // TODO: Implementar autenticación delegada a AWS IAM / Cognito.
                // El secreto se leerá desde los User Secrets del proyecto
                // bajo la variable "IAMSecret".
                //
                // Ejemplo de estructura futura:
                //   var iamSecret = _configuration["IAMSecret"];
                //   var cognitoClient = new AmazonCognitoIdentityProviderClient(...);
                //   var authRequest = new InitiateAuthRequest
                //   {
                //       AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
                //       ClientId  = iamSecret,   // o el App Client ID de Cognito
                //       AuthParameters = new Dictionary<string, string>
                //       {
                //           { "USERNAME", dto.Username },
                //           { "PASSWORD", dto.Password }
                //       }
                //   };
                //   var result = await cognitoClient.InitiateAuthAsync(authRequest);
                //   // Mapear result.AuthenticationResult.IdToken → AuthResponseDTO
                // ──────────────────────────────────────────────────────────────
                throw new NotImplementedException(
                    "Autenticación AWS IAM no implementada aún. " +
                    "Configure UseAwsIam=false para usar JWT local.");
            }

            // ── JWT local: verificar hash SHA-256 ─────────────────────────────
            var user = await _context.Users
                .Include(u => u.Person)
                .FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (user == null)
                return null;

            var hashedInput = HashPassword(dto.Password);
            if (user.PasswordHash != hashedInput)
                return null;

            return BuildAuthResponse(user);
        }

        // ──────────────────────────────────────────────────────────────────────
        // Registro unificado — crea Person + User en una transacción atómica
        // ──────────────────────────────────────────────────────────────────────

        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO dto)
        {
            // Verificar unicidad de username y email
            var usernameTaken = await _context.Users
                .AnyAsync(u => u.Username == dto.Username);
            if (usernameTaken)
                throw new InvalidOperationException($"El username '{dto.Username}' ya está en uso.");

            var emailTaken = await _context.Users
                .AnyAsync(u => u.Email == dto.Email);
            if (emailTaken)
                throw new InvalidOperationException($"El email '{dto.Email}' ya está registrado.");

            var useAwsIam = _configuration.GetValue<bool>("UseAwsIam");

            // ── Transacción atómica ────────────────────────────────────────────
            await using var transaction = await _context.Database.BeginTransactionAsync();

            // 1. Crear Person
            var person = new Persons
            {
                FirstName            = dto.FirstName,
                LastName             = dto.LastName,
                MiddleName           = dto.MiddleName,
                DateOfBirth          = dto.DateOfBirth,
                IdentificationNumber = dto.IdentificationNumber,
                Phone                = dto.Phone,
                Address              = dto.Address,
                CreatedAt            = DateTime.UtcNow
            };
            _context.Persons.Add(person);
            await _context.SaveChangesAsync();

            // 2. Crear User
            var user = new Users
            {
                Username  = dto.Username,
                Email     = dto.Email,
                PersonId  = person.Id,
                CreatedAt = DateTime.UtcNow,

                // PasswordHash es null si UseAwsIam = true (IAM gestiona la autenticación)
                // PasswordHash contiene el SHA-256 si UseAwsIam = false (JWT local)
                PasswordHash = useAwsIam ? null : HashPassword(dto.Password)
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            user.Person = person;
            return BuildAuthResponse(user);
        }

        // ──────────────────────────────────────────────────────────────────────
        // Helpers privados
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Genera el JWT firmado con la clave de JwtSettings.
        /// La expiración se toma de JwtSettings:ExpiresInHours (por defecto 8 h).
        /// </summary>
        private AuthResponseDTO BuildAuthResponse(Users user)
        {
            var jwtKey     = _configuration["JwtSettings:Key"]!;
            var issuer     = _configuration["JwtSettings:Issuer"]!;
            var audience   = _configuration["JwtSettings:Audience"]!;
            var expiresIn  = _configuration.GetValue<int>("JwtSettings:ExpiresInHours", 8);

            var key        = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds      = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(expiresIn);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("username",                    user.Username),
                new Claim(JwtRegisteredClaimNames.Jti,  Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer:             issuer,
                audience:           audience,
                claims:             claims,
                expires:            expiration,
                signingCredentials: creds
            );

            return new AuthResponseDTO
            {
                Token     = new JwtSecurityTokenHandler().WriteToken(token),
                Username  = user.Username,
                Email     = user.Email,
                ExpiresAt = expiration
            };
        }

        /// <summary>
        /// Hashea una contraseña con SHA-256 y devuelve el resultado en hex minúscula.
        /// </summary>
        private static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }
    }
}
