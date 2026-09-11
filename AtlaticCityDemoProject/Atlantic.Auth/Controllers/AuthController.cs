using Atlantic.Auth.Models;
using Atlantic.Auth.Services;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Atlantic.Auth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly PasswordHasher<Usuario> _passwordHasher;
        private readonly ITokenService _tokenService;
        public AuthController(IConfiguration config, ITokenService tokenService)
        {
            _config = config;
            _passwordHasher = new PasswordHasher<Usuario>();
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthRequest request)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var userExists = await connection.ExecuteScalarAsync<bool>(
                "SELECT COUNT(1) FROM Usuarios WHERE Correo = @Correo", new { request.Correo });

            if (userExists) 
                return BadRequest(new { mensaje = "El correo ya está registrado." });

            var hash = _passwordHasher.HashPassword(new Usuario(), request.Password);

            var query = "INSERT INTO Usuarios (Correo, PasswordHash, Rol) VALUES (@Correo, @PasswordHash, @Rol)";
            await connection.ExecuteAsync(query, new { request.Correo, PasswordHash = hash, request.Rol });

            return Ok(new { mensaje = "Usuario registrado exitosamente." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequest request)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var query = "SELECT * FROM Usuarios WHERE Correo = @Correo";
            var usuario = await connection.QuerySingleOrDefaultAsync<Usuario>(query, new { request.Correo });

            if (usuario == null) 
                return Unauthorized(new { mensaje = "Credenciales incorrectas" });

            var result = _passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed)
                return Unauthorized(new { mensaje = "Credenciales incorrectas" });

            var token = _tokenService.GenerarJwt(usuario);
            var refreshToken = _tokenService.GenerarRefreshToken();

            var updateQuery = @"UPDATE Usuarios 
                                SET RefreshToken = @RefreshToken, RefreshTokenExpiryTime = @ExpiryTime 
                                WHERE Id = @Id";

            await connection.ExecuteAsync(updateQuery, new
            {
                RefreshToken = refreshToken,
                ExpiryTime = DateTime.UtcNow.AddHours(1),
                Id = usuario.Id
            });

            return Ok(new { Token = token, RefreshToken = refreshToken });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var query = "SELECT * FROM Usuarios WHERE RefreshToken = @RefreshToken";
            var usuario = await connection.QuerySingleOrDefaultAsync<Usuario>(query, new { request.RefreshToken });

            if (usuario == null || usuario.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return Unauthorized(new { mensaje = "Refresh token inválido o expirado. Inicie sesión nuevamente." });

            var newJwtToken = _tokenService.GenerarJwt(usuario);
            var newRefreshToken = _tokenService.GenerarRefreshToken();

            var updateQuery = @"UPDATE Usuarios 
                                SET RefreshToken = @RefreshToken, RefreshTokenExpiryTime = @ExpiryTime 
                                WHERE Id = @Id";
            await connection.ExecuteAsync(updateQuery, new
            {
                RefreshToken = newRefreshToken,
                ExpiryTime = DateTime.UtcNow.AddMinutes(1),
                Id = usuario.Id
            });

            return Ok(new { Token = newJwtToken, RefreshToken = newRefreshToken });
        }
    }
}
