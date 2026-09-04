using cloudinvoice_web_ui.DTOs.Identity;

namespace cloudinvoice_web_ui.Services.Identity
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest);
    }
}