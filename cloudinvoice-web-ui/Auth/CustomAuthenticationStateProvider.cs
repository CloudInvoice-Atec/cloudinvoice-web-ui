using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace cloudinvoice_web_ui.Auth // Ajusta para o teu namespace correto
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;
        private const string TokenKey = "authToken"; // A chave que usas para guardar o token no localStorage

        public CustomAuthenticationStateProvider(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        // Este método é chamado pelo Blazor para saber quem é o utilizador atual
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string? token = null;
            try
            {
                // Tenta ir buscar o token JWT ao localStorage do browser
                token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey);
            }
            catch
            {
                // Ignora se o JS interop falhar (ex: durante o pré-render no servidor)
            }

            // Se não houver token, o utilizador está anónimo (não logado)
            if (string.IsNullOrWhiteSpace(token))
            {
                var anonymousIdentity = new ClaimsIdentity();
                var anonymousUser = new ClaimsPrincipal(anonymousIdentity);
                return new AuthenticationState(anonymousUser);
            }

            // Se houver token, criamos a identidade com base nas claims do JWT
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
            return new AuthenticationState(authenticatedUser);
        }

        // Método auxiliar para notificar a aplicação que o utilizador fez login (para atualizar a UI de imediato)
        public void MarkUserAsAuthenticated(string token)
        {
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        // Método auxiliar para notificar que o utilizador fez logout
        public void MarkUserAsLoggedOut()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }

        // Função para descodificar o Payload de um token JWT e extrair as Claims (Email, Role, Nome, etc.)
        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            var claims = new List<Claim>();
            if (keyValuePairs != null)
            {
                foreach (var kvp in keyValuePairs)
                {
                    if (kvp.Value is JsonElement element && element.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var val in element.EnumerateArray())
                        {
                            claims.Add(new Claim(kvp.Key, val.ToString() ?? ""));
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(kvp.Key, kvp.Value.ToString() ?? ""));
                    }
                }
            }
            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}