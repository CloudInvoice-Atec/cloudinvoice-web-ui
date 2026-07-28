namespace cloudinvoice_web_ui.Auth
{
    /// <summary>
    /// Serviço scoped que guarda o token JWT em memória no circuito Blazor Server.
    /// Evita a necessidade de usar IJSRuntime (localStorage) fora de componentes.
    /// </summary>
    public class TokenProvider
    {
        public string? Token { get; set; }
    }
}
