using cloudinvoice_web_ui.DTOs.Clientes;
using cloudinvoice_web_ui.DTOs.Invoices;
using cloudinvoice_web_ui.Auth;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace cloudinvoice_web_ui.Services.Customers
{
    public class CustomerService : ICustomerService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TokenProvider _tokenProvider;
        private readonly HttpClient _httpClientBilling; // <-- Declarado no topo como na Empresa

        // O construtor injeta as dependências e prepara logo o HttpClient
        public CustomerService(IHttpClientFactory httpClientFactory, TokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
            _httpClientBilling = CreateAuthenticatedClient("BillingAPI"); // <-- Instanciado no início
        }

        // Método auxiliar idêntico ao do CompanyService
        private HttpClient CreateAuthenticatedClient(string clientName)
        {
            var client = _httpClientFactory.CreateClient(clientName);
            if (!string.IsNullOrEmpty(_tokenProvider.Token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenProvider.Token);
            }
            return client;
        }

        public async Task<Guid?> CreateCustomerAsync(InserirClienteDto customer)
        {
            try
            {
                var response = await _httpClientBilling.PostAsJsonAsync("api/Customers", customer);

                if (response.IsSuccessStatusCode)
                {
                    var createdCustomer = await response.Content.ReadFromJsonAsync<CustomerProfileDto>();
                    return createdCustomer?.Id;
                }

                Console.WriteLine($"Erro da API ao criar cliente. Status: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar cliente: {ex.Message}");
                return null;
            }
        }

        public async Task<CustomerProfileDto> GetCustomerProfileAsync(Guid id)
        {
            try
            {
                var result = await _httpClientBilling.GetFromJsonAsync<CustomerProfileDto>($"api/customers/{id}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter cliente da API: {ex.Message}.");
                return null;
            }
        }

        public async Task<List<InvoiceSummaryDto>> GetCustomerInvoicesAsync(Guid id)
        {
            try
            {
                // Usamos diretamente o _httpClientBilling instanciado no construtor
                var invoices = await _httpClientBilling.GetFromJsonAsync<List<InvoiceSummaryDto>>($"api/customers/{id}/invoices");

                return invoices ?? new List<InvoiceSummaryDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter faturas da API: {ex.Message}.");
                return new List<InvoiceSummaryDto>();
            }
        }

        public async Task<bool> UpdateCustomerAsync(Guid id, CustomerProfileDto customer)
        {
            try
            {
                // Usamos diretamente o _httpClientBilling instanciado no construtor
                var response = await _httpClientBilling.PutAsJsonAsync($"api/customers/{id}", customer);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar cliente: {ex.Message}");
                return false;
            }
        }

        public async Task<PagedResultDto<CustomerProfileDto>?> GetCustomersAsync(CustomerQueryParameters parameters)
        {
            try
            {
                var queryParams = new List<string>
        {
            $"page={parameters.Page}",
            $"pageSize={parameters.PageSize}"
        };

                if (!string.IsNullOrWhiteSpace(parameters.Search))
                {
                    queryParams.Add($"search={Uri.EscapeDataString(parameters.Search)}");
                }

                if (parameters.IsActive.HasValue)
                {
                    queryParams.Add($"isActive={parameters.IsActive.Value}");
                }

                var queryString = string.Join("&", queryParams);

                return await _httpClientBilling.GetFromJsonAsync<PagedResultDto<CustomerProfileDto>>($"api/customers?{queryString}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter clientes paginados: {ex.Message}");
                return null;
            }
        }

        public async Task<List<CustomerProfileDto>> GetCustomersActiveAsync()
        {
            try
            {
                var clientesAtivos = await _httpClientBilling.GetFromJsonAsync<List<CustomerProfileDto>>("api/customers/active");
                return clientesAtivos ?? new List<CustomerProfileDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter clientes: {ex.Message}");
                return new List<CustomerProfileDto>();
            }
        }
    }
}