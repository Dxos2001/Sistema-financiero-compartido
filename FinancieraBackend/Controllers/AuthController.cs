using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinancieraBackend.Domain.Interfaces;
using FinancieraBackend.Domain.DTOs;

namespace FinancieraBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Autentica al usuario y devuelve un JWT.
        /// Endpoint público — no requiere autenticación previa.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> Login([FromBody] LoginDTO dto)
        {
            var result = await _authService.AuthenticateAsync(dto);

            if (result == null)
                return Unauthorized(new { Message = "Usuario o contraseña incorrectos." });

            return Ok(result);
        }

        /// <summary>
        /// Registra un nuevo usuario (crea Person + User) y devuelve un JWT.
        /// Endpoint público — no requiere autenticación previa.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDTO>> Register([FromBody] RegisterDTO dto)
        {
            try
            {
                var result = await _authService.RegisterAsync(dto);
                return CreatedAtAction(nameof(Login), result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
        }
    }
}
