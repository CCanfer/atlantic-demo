using Atlantic.Auth.Models;

namespace Atlantic.Auth.Services
{
    public interface ITokenService
    {
        string GenerarJwt(Usuario usuario);
        string GenerarRefreshToken();
    }
}
