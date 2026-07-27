using System.Net.Http.Json;
using cloudinvoice_web_ui.Models.Auth;

namespace Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest loginRequest)
        {
            try
            {
                // Cria o client nomeado configurado no Program.cs (aponta para https://localhost:5001/)
                var client = _httpClientFactory.CreateClient("IdentityAPI");

                var response = await client.PostAsJsonAsync("api/Auth/login", loginRequest);
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

                if (result != null)
                {
                    return result;
                }

                return new AuthResponse { IsSuccess = false, Message = "Erro ao processar a resposta do servidor." };
            }
            catch (Exception ex)
            {
                return new AuthResponse { IsSuccess = false, Message = $"Erro de comunicação com a API: {ex.Message}" };
            }
        }
    }
}