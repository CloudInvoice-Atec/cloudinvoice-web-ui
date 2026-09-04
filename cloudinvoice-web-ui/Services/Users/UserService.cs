using cloudinvoice_web_ui.Auth;
using cloudinvoice_web_ui.DTOs.Identity;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.JSInterop;

namespace cloudinvoice_web_ui.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TokenProvider _tokenProvider;
        private readonly HttpClient _httpClientIdentity;
        private readonly IJSRuntime _jsRuntime;

        public UserService(IHttpClientFactory httpClientFactory, TokenProvider tokenProvider, IJSRuntime jsRuntime)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
            _jsRuntime = jsRuntime;

            // Aponta estritamente para a IdentityAPI
            _httpClientIdentity = CreateAuthenticatedClient("IdentityAPI");
        }

        private HttpClient CreateAuthenticatedClient(string clientName)
        {
            var client = _httpClientFactory.CreateClient(clientName);
            if (!string.IsNullOrEmpty(_tokenProvider.Token)) // Confirma se no teu TokenProvider a propriedade é JwtToken ou Token
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenProvider.Token);
            }
            return client;
        }

        public async Task<List<UserResponseDto>> GetUsersAsync()
        {
            try
            {
                // Faz o GET ao endpoint que acabaste de criar na API
                var users = await _httpClientIdentity.GetFromJsonAsync<List<UserResponseDto>>("api/users");

                return users ?? new List<UserResponseDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter a lista de utilizadores: {ex.Message}");
                // Retorna nulo para a UI saber que houve uma falha de comunicação e apresentar erro
                return null;
            }
        }

        public async Task<AuthResponseDto> RegisterUserAsync(RegisterRequestDto registerDto)
        {
            try
            {
                var response = await _httpClientIdentity.PostAsJsonAsync("api/Auth/register", registerDto);
                var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

                if (result != null)
                {
                    return result;
                }

                return new AuthResponseDto { IsSuccess = false, Message = "Erro ao processar a resposta do servidor." };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao registar utilizador: {ex.Message}");
                return new AuthResponseDto { IsSuccess = false, Message = $"Erro de comunicação: {ex.Message}" };
            }
        }

        public async Task<(bool Success, string Message)> DeleteUserAsync(string id)
        {
            var response = await _httpClientIdentity.DeleteAsync($"api/users/{id}");

            if (response.IsSuccessStatusCode)
            {
                return (true, "Utilizador eliminado com sucesso.");
            }

            // Capture the error message from the response
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                var errorResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
                var message = errorResponse?["message"]?.ToString() ?? "Erro ao eliminar utilizador.";
                return (false, message);
            }
            catch
            {
                return (false, "Erro ao eliminar utilizador.");
            }
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(string id)
        {
            try
            {
                // 1. Ir buscar o token guardado (via JSRuntime/localStorage ou TokenProvider)
                // Nota: Precisas de ter o IJSRuntime injetado no construtor do teu UserService
                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

                // 2. Adicionar o token ao cabeçalho de Autorização do HttpClient
                if (!string.IsNullOrWhiteSpace(token))
                {
                    _httpClientIdentity.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                // 3. Fazer o pedido à API já com o token incluído
                return await _httpClientIdentity.GetFromJsonAsync<UserResponseDto>($"api/Users/{id}");
            }
            catch (Exception ex)
            {
                // Opcional: Podes fazer um Console.WriteLine(ex.Message) aqui para veres no F12 se houver outros erros
                return null;
            }
        }

        public async Task<bool> UpdateUserAsync(UserResponseDto user)
        {
            try
            {
                var response = await _httpClientIdentity.PutAsJsonAsync($"api/Users/{user.Id}", user);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar utilizador: {ex.Message}");
                return false;
            }
        }
    }
}