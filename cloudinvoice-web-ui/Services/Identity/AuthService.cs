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
    }
}