using cloudinvoice_web_ui.DTOs.Dashboard;
using cloudinvoice_web_ui.Auth;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace cloudinvoice_web_ui.Services.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TokenProvider _tokenProvider;
        private readonly HttpClient _httpClientBilling;

        public DashboardService(IHttpClientFactory httpClientFactory, TokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
            _httpClientBilling = CreateAuthenticatedClient("BillingAPI");
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

        public async Task<DashboardOverviewDto?> GetDashboardOverviewAsync()
        {
            try
            {
                // Devolve os dados reais da API. Se falhar, o bloco catch apanha a exceção.
                return await _httpClientBilling.GetFromJsonAsync<DashboardOverviewDto>("api/dashboard/overview");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter dados do dashboard: {ex.Message}");
                // Regra 2: Sem mocks. Devolvemos null e a UI que lide com o erro.
                return null;
            }
        }
    }
}
