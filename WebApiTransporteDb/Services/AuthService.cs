using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using WebApiTransporteDb.Models;

namespace WebApiTransporteDb.Services
{
    public class AuthService
    {
        private readonly string _connectionString;
        private readonly IConfiguration _config;

        public AuthService(IConfiguration config)
        {
            _config = config;
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        /// <summary>
        /// Crea o actualiza el usuario admin al iniciar la aplicación.
        /// </summary>
        public void SeedAdmin()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                var hash = BCrypt.Net.BCrypt.HashPassword("admin123", 11);

                var existe = connection.QueryFirstOrDefault<int>(
                    "SELECT COUNT(1) FROM Usuarios WHERE Username = 'admin'"
                );

                if (existe == 0)
                {
                    connection.Execute(
                        "INSERT INTO Usuarios (Username, PasswordHash, Rol) VALUES (@Username, @PasswordHash, @Rol)",
                        new { Username = "admin", PasswordHash = hash, Rol = "Admin" }
                    );
                }
                else
                {
                    // Actualizar hash para asegurar que siempre sea correcto
                    connection.Execute(
                        "UPDATE Usuarios SET PasswordHash = @PasswordHash WHERE Username = 'admin'",
                        new { PasswordHash = hash }
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SeedAdmin] Error: {ex.Message}");
            }
        }

        public (string? Token, string? Error) Login(string username, string password)
        {
            using var connection = new SqlConnection(_connectionString);

            var usuario = connection.QueryFirstOrDefault<Usuario>(
                "SELECT * FROM Usuarios WHERE Username = @Username AND Activo = 1",
                new { Username = username }
            );

            if (usuario == null)
                return (null, "Usuario no encontrado.");

            bool passwordValida = BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash);
            if (!passwordValida)
                return (null, "Contraseña incorrecta.");

            var token = GenerarToken(usuario);
            return (token, null);
        }

        public (bool Success, string Message) Registrar(string username, string password, string rol = "User")
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                // Verificar si ya existe
                var existe = connection.QueryFirstOrDefault<int>(
                    "SELECT COUNT(1) FROM Usuarios WHERE Username = @Username",
                    new { Username = username }
                );

                if (existe > 0)
                    return (false, "El nombre de usuario ya está en uso.");

                // Hashear y guardar
                var hash = BCrypt.Net.BCrypt.HashPassword(password, 11);
                
                connection.Execute(
                    "INSERT INTO Usuarios (Username, PasswordHash, Rol) VALUES (@Username, @PasswordHash, @Rol)",
                    new { Username = username, PasswordHash = hash, Rol = rol }
                );

                return (true, "Usuario registrado exitosamente.");
            }
            catch (Exception ex)
            {
                return (false, $"Error al registrar usuario: {ex.Message}");
            }
        }

        private string GenerarToken(Usuario usuario)
        {
            var jwtKey = _config["Jwt:Key"]!;
            var issuer  = _config["Jwt:Issuer"]!;
            var audience = _config["Jwt:Audience"]!;
            var expiresHours = int.Parse(_config["Jwt:ExpiresHours"] ?? "8");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim(ClaimTypes.Role, usuario.Rol),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer:   issuer,
                audience: audience,
                claims:   claims,
                expires:  DateTime.UtcNow.AddHours(expiresHours),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
