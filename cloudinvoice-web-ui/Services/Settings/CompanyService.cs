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
                // Usa o client adequado (ex: IdentityAPI, CoreAPI, etc.)
                var client = _httpClientFactory.CreateClient("BillingAPI");

                var empresa = await client.GetFromJsonAsync<EmpresaDto>("api/settings/company");
                if (empresa != null)
                {
                    return empresa;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter definições da empresa: {ex.Message}. A carregar dados fictícios.");
            }

            // FAKE DATA de Fallback para a página renderizar sem erros
            return new EmpresaDto
            {
                Name = "A Minha Empresa Fictícia, Lda",
                TaxNumber = "500999888",
                Address = "Rua do Comércio, 123",
                City = "Guimarães",
                PostalCode = "4800-123",
                Country = "Portugal",
                Email = "geral@minhaempresa.pt",
                Phone = "+351 253 000 000",
                Website = "https://www.minhaempresa.pt",
                RegistryOffice = "Conservatória de Guimarães",
                CommercialRegistrationNumber = "500999888",
                ShareCapital = 50000,
                BankName = "Banco Fictício",
                Iban = "PT50 0000 0000 1234 5678 901 23",
                Swift = "BFPTPTPL"
            };
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