using System.Net.Http.Headers;
using cloudinvoice_web_ui.Auth;

public class JwtAuthorizationHandler : DelegatingHandler
{
    private readonly TokenProvider _tokenProvider;

    public JwtAuthorizationHandler(TokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Lê o token do TokenProvider (guardado em memória no circuito Blazor)
        var token = _tokenProvider.Token;

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
