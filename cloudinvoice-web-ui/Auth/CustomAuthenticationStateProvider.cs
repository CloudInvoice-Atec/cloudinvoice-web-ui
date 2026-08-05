using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace cloudinvoice_web_ui.Auth
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly TokenProvider _tokenProvider;
        private const string TokenKey = "authToken";

        public CustomAuthenticationStateProvider(IJSRuntime jsRuntime, TokenProvider tokenProvider)
        {
            _jsRuntime = jsRuntime;
            _tokenProvider = tokenProvider;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string? token = _tokenProvider.Token;

            // Se o TokenProvider ainda não tem o token, tenta ir buscar ao localStorage
            if (string.IsNullOrWhiteSpace(token))
            {
                try
                {
                    token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey);
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        // Repõe o token na memória para usos futuros rápidos
                        _tokenProvider.Token = token;
                    }
                }
                catch
                {
                    // Ignora se o JS interop falhar (ex: durante o pré-render no servidor)
                }
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt", "name", "role");
            var authenticatedUser = new ClaimsPrincipal(identity);
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