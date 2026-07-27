using cloudinvoice_web_ui.DTOs.Configuracoes;

namespace cloudinvoice_web_ui.Services.Settings
{
    public class CompanyService : ICompanyService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CompanyService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<EmpresaDto> GetCompanySettingsAsync()
        {
            try
            {
                // Ajusta "IdentityAPI" ou "BillingAPI" conforme o nome que deste no Program.cs
                var client = _httpClientFactory.CreateClient("BillingAPI");

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
                var client = _httpClientFactory.CreateClient("IdentityAPI");

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