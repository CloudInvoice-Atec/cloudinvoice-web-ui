using cloudinvoice_web_ui.Models.Auth;

namespace cloudinvoice_web_ui.Services.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest loginRequest);
    }
}