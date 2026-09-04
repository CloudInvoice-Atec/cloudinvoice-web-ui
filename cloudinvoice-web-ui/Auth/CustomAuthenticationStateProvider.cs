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

        // 🚨 ALTERAÇÃO: Passou a ser 'async Task' e agora GRAVA no localStorage
        public async Task MarkUserAsAuthenticated(string token)
        {
            // 1. Atualiza na memória
            _tokenProvider.Token = token;

            // 2. Grava no disco do browser para sobreviver ao F5!
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, token);

            // 3. Notifica a aplicação
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        // 🚨 ALTERAÇÃO: Passou a ser 'async Task' e agora APAGA do localStorage
        public async Task MarkUserAsLoggedOut()
        {
            // 1. Limpa a memória
            _tokenProvider.Token = null;

            // 2. Apaga do disco do browser
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);

            // 3. Notifica a aplicação
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }

        // Função para descodificar o Payload de um token JWT e extrair as Claims (Email, Role, Nome, etc.)
        // Função melhorada para descodificar o JWT e garantir que a Role é mapeada corretamente
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
                    // Mapeamento inteligente: Se a chave for qualquer variante de 'role', garantimos que criamos um Claim do tipo ClaimTypes.Role
                    var claimType = kvp.Key;
                    if (claimType.Equals("role", StringComparison.OrdinalIgnoreCase) ||
                        claimType.Equals("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", StringComparison.OrdinalIgnoreCase))
                    {
                        claimType = ClaimTypes.Role;
                    }

                    if (kvp.Value is JsonElement element && element.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var val in element.EnumerateArray())
                        {
                            claims.Add(new Claim(claimType, val.ToString() ?? ""));
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(claimType, kvp.Value.ToString() ?? ""));
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

        public async Task LoadTokenFromBrowserAsync()
        {
            try
            {
                // Neste momento o SignalR já está ligado, o JS Interop vai funcionar a 100%
                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey);

                // Se encontrou o token no disco e a memória estava vazia (F5), repõe a sessão
                if (!string.IsNullOrWhiteSpace(token) && _tokenProvider.Token != token)
                {
                    _tokenProvider.Token = token;

                    var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt", "name", "role");
                    var authenticatedUser = new ClaimsPrincipal(identity);
                    var authState = Task.FromResult(new AuthenticationState(authenticatedUser));

                    // Força toda a aplicação a atualizar-se e a remover o estado Anónimo da cache
                    NotifyAuthenticationStateChanged(authState);
                }
            }
            catch
            {
                // Ignora falhas de segurança do browser
            }
        }
    }
}