using Microsoft.AspNetCore.Mvc;
using WebApiTransporteDb.Models;
using WebApiTransporteDb.Services;

namespace WebApiTransporteDb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Inicia sesión y devuelve un JWT.
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { mensaje = "Usuario y contraseña son obligatorios." });

            var (token, error) = _authService.Login(dto.Username, dto.Password);

            if (token == null)
                return Unauthorized(new { mensaje = error });

            return Ok(new
            {
                token,
                username = dto.Username,
                mensaje = "Sesión iniciada correctamente."
            });
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema.
        /// </summary>
        [HttpPost("registro")]
        public IActionResult Registro([FromBody] LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { mensaje = "Usuario y contraseña son obligatorios." });

            var (success, message) = _authService.Registrar(dto.Username, dto.Password);

            if (!success)
                return BadRequest(new { mensaje = message });

            return Ok(new { mensaje = message });
        }
    }
}
