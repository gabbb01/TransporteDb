namespace WebApiTransporteDb.Models
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Rol { get; set; } = "Admin";
        public bool Activo { get; set; } = true;
    }
}
