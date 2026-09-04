using cloudinvoice_web_ui.DTOs.Identity;
using System.Net.Http.Json;

namespace cloudinvoice_web_ui.Services.Identity
{
    public class AuthService : IAuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpCustomerIdentity;

        public AuthService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            // Cria o cliente nomeado configurado para a Identity.API
            _httpCustomerIdentity = _httpClientFactory.CreateClient("IdentityAPI");
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            try
            {
                var response = await _httpCustomerIdentity.PostAsJsonAsync("api/Auth/login", loginRequest);
                var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

                if (result != null)
                {
                    return result;
                }

                return new AuthResponseDto { IsSuccess = false, Message = "Erro ao processar a resposta do servidor." };
            }
            catch (Exception ex)
            {
                return new AuthResponseDto { IsSuccess = false, Message = $"Erro de comunicação com a API: {ex.Message}" };
            }
        }

        public async Task ForgotPasswordAsync(string email)
        {
            try
            {
                // backend should always return 200 to avoid user enumeration
                await _httpClientIdentity.PostAsJsonAsync("api/auth/forgot-password", new { Email = email });
            }
            catch
            {
                // swallow exceptions to preserve UX; consider logging in production
            }
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto model)
        {
            try
            {
                var resp = await _httpClientIdentity.PostAsJsonAsync("api/auth/reset-password", model);
                return resp.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
