using cloudinvoice_web_ui.Models.Auth;

namespace Services.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest loginRequest);
    }
}