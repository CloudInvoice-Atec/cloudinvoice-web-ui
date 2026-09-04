using cloudinvoice_web_ui.DTOs.Identity;

namespace cloudinvoice_web_ui.Services.Users
{
    public interface IUserService
    {
        // Método para listar os utilizadores
        Task<List<UserResponseDto>> GetUsersAsync();

        // Método para criar/registar um utilizador (já o tínhamos)
        Task<AuthResponseDto> RegisterUserAsync(RegisterRequestDto registerDto);

        Task<(bool Success, string Message)> DeleteUserAsync(string id);

        Task<UserResponseDto?> GetUserByIdAsync(string id);
        Task<bool> UpdateUserAsync(UserResponseDto user);
    }
}