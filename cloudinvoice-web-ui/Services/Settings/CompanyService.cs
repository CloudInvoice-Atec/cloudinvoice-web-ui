using cloudinvoice_web_ui.DTOs.Configuracoes;
using cloudinvoice_web_ui.Auth;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting; // 1. OBRIGATÓRIO: Para aceder às pastas físicas do servidor

namespace cloudinvoice_web_ui.Services.Settings
{
    public class CompanyService : ICompanyService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TokenProvider _tokenProvider;
        private readonly HttpClient _httpClientBilling;
        private readonly IWebHostEnvironment _env; // Variável para as pastas

        // 2. Injetamos o IWebHostEnvironment no construtor
        public CompanyService(
            IHttpClientFactory httpClientFactory,
            TokenProvider tokenProvider,
            IWebHostEnvironment env)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
            _httpClientBilling = CreateAuthenticatedClient("BillingAPI");
            _env = env;
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
                return await _httpClientBilling.GetFromJsonAsync<EmpresaDto>("api/Companies/1");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter dados da empresa: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> SaveCompanySettingsAsync(EmpresaDto empresa)
        {
            try
            {
                

                string novoNomeFicheiro = null;

                if (empresa.LogoFile != null)
                {
                    var extensao = Path.GetExtension(empresa.LogoFile.Name);

                    novoNomeFicheiro = $"{Guid.NewGuid()}{extensao}";

                    empresa.Logo = $"/uploads/{novoNomeFicheiro}";
                }

                var response = await _httpClientBilling.PutAsJsonAsync("api/Companies/1", empresa);

                if (response.IsSuccessStatusCode && empresa.Logo != null)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var filePath = Path.Combine(uploadsFolder, novoNomeFicheiro);

                    using var streamDeSaida = new FileStream(filePath, FileMode.Create);
                    await empresa.LogoFile.OpenReadStream(10485760).CopyToAsync(streamDeSaida);
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao gravar definições da empresa: {ex.Message}");
                return false;
            }
        }
    }
}