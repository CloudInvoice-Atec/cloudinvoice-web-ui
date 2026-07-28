using System.Net.Http.Headers;
using Microsoft.JSInterop; // Se guardares o token no LocalStorage

public class JwtAuthorizationHandler : DelegatingHandler
{
    private readonly IJSRuntime _jsRuntime;

    public JwtAuthorizationHandler(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string? token = null;
        try
        {
            // 1. Vais buscar o token JWT ao LocalStorage do browser
            token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
        }
        catch (InvalidOperationException)
        {
            // Ignora se o JS interop falhar (ex: durante o pré-render no servidor)
        }

        // 2. Se o token existir, adicionas-o ao cabeçalho do pedido
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // 3. Deixa o pedido seguir viagem para a API
        return await base.SendAsync(request, cancellationToken);
    }
}
