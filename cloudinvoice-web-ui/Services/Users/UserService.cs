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
        private readonly HttpClient _httpCustomerIdentity;
        private readonly IJSRuntime _jsRuntime;

        public UserService(IHttpClientFactory httpClientFactory, TokenProvider tokenProvider, IJSRuntime jsRuntime)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
            _jsRuntime = jsRuntime;

            
            _httpCustomerIdentity = CreateAuthenticatedClient("IdentityAPI");
        }

        private HttpClient CreateAuthenticatedClient(string clientName)
        {
            var client = _httpClientFactory.CreateClient(clientName);
            if (!string.IsNullOrEmpty(_tokenProvider.Token)) 
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenProvider.Token);
            }
            return client;
        }

        public async Task<List<UserResponseDto>> GetUsersAsync()
        {
            try
            {
                
                var users = await _httpCustomerIdentity.GetFromJsonAsync<List<UserResponseDto>>("api/users");

                return users ?? new List<UserResponseDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter a lista de utilizadores: {ex.Message}");
                
                return null;
            }
        }

        public async Task<AuthResponseDto> RegisterUserAsync(RegisterRequestDto registerDto)
        {
            try
            {
                var response = await _httpCustomerIdentity.PostAsJsonAsync("api/Auth/register", registerDto);
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
            var response = await _httpCustomerIdentity.DeleteAsync($"api/users/{id}");

            if (response.IsSuccessStatusCode)
            {
                return (true, "Utilizador eliminado com sucesso.");
            }

            
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
                
                
                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

                
                if (!string.IsNullOrWhiteSpace(token))
                {
                    _httpCustomerIdentity.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                
                return await _httpCustomerIdentity.GetFromJsonAsync<UserResponseDto>($"api/Users/{id}");
            }
            catch (Exception ex)
            {
                
                return null;
            }
        }

        public async Task<bool> UpdateUserAsync(UserResponseDto user)
        {
            try
            {
                var response = await _httpCustomerIdentity.PutAsJsonAsync($"api/Users/{user.Id}", user);
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