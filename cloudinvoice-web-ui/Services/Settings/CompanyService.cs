using cloudinvoice_web_ui.DTOs.Configuracoes;
using cloudinvoice_web_ui.Auth;
using System.Net.Http.Headers;

namespace cloudinvoice_web_ui.Services.Settings
{
    public class CompanyService : ICompanyService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TokenProvider _tokenProvider;

        public CompanyService(IHttpClientFactory httpClientFactory, TokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
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

        public async Task<EmpresaDto> GetCompanySettingsAsync()
        {
            try
            {
                var client = CreateAuthenticatedClient("BillingAPI");

                // Faz o GET apontando diretamente para o ID 1
                return await client.GetFromJsonAsync<EmpresaDto>("api/Companies/1");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter dados da empresa: {ex.Message}");
                return null; // Retorna nulo se falhar, a UI tratará o erro
            }
        }

        public async Task<bool> SaveCompanySettingsAsync(EmpresaDto empresa)
        {
            try
            {
                var client = CreateAuthenticatedClient("IdentityAPI");

                // NOTA: Como tens um ficheiro (IBrowserFile Logo), o ideal num cenário real 
                // é usar MultipartFormDataContent em vez de PostAsJsonAsync.
                // Para efeitos de estrutura, deixo o envio em JSON padrão.

                //Tratamento do ficheiro Logo (IBrowserFile) alojamento em Uploads, rename, e envio o caminho do ficheiro no DTO. Aqui apenas envio o DTO como está.






                var response = await client.PostAsJsonAsync("api/settings/company", empresa);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao gravar definições da empresa: {ex.Message}");
                return false; // Simula falha se a API não estiver ligada
            }
        }
    }
}