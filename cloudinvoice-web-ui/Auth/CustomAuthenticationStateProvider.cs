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

            
            if (string.IsNullOrWhiteSpace(token))
            {
                try
                {
                    token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey);
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        
                        _tokenProvider.Token = token;
                    }
                }
                catch
                {
                    
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

        
        public async Task MarkUserAsAuthenticated(string token)
        {
            
            _tokenProvider.Token = token;

            
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, token);

            
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        
        public async Task MarkUserAsLoggedOut()
        {
            
            _tokenProvider.Token = null;

            
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);

            
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }

        
        
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
                
                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey);

                
                if (!string.IsNullOrWhiteSpace(token) && _tokenProvider.Token != token)
                {
                    _tokenProvider.Token = token;

                    var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt", "name", "role");
                    var authenticatedUser = new ClaimsPrincipal(identity);
                    var authState = Task.FromResult(new AuthenticationState(authenticatedUser));

                    
                    NotifyAuthenticationStateChanged(authState);
                }
            }
            catch
            {
                
            }
        }
    }
}