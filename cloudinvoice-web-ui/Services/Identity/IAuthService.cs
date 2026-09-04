using cloudinvoice_web_ui.DTOs.Identity;

namespace cloudinvoice_web_ui.Services.Identity
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest);

        // Send forgot-password email (always return 200 semantics handled by backend)
        Task ForgotPasswordAsync(string email);

        // Reset password using token
        Task<bool> ResetPasswordAsync(ResetPasswordDto model);
    }
}
