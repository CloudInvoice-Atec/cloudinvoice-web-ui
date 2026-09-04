using cloudinvoice_web_ui.Auth;
using cloudinvoice_web_ui.DTOs.Invoices;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

namespace cloudinvoice_web_ui.Services.Invoices
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TokenProvider _tokenProvider;
        private readonly HttpClient _httpClientBilling;
        private readonly HttpClient _httpClientCatalog;
        private readonly AuthenticationStateProvider _authStateProvider;

        public InvoiceService(IHttpClientFactory httpClientFactory, TokenProvider tokenProvider, AuthenticationStateProvider authStateProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
            _httpClientBilling = _httpClientFactory.CreateClient("BillingAPI");
            _httpClientCatalog = _httpClientFactory.CreateClient("CatalogAPI");
            _authStateProvider = authStateProvider;
        }

        private HttpClient CreateAuthenticatedClient()
        {
            var client = _httpClientFactory.CreateClient("BillingAPI");
            if (!string.IsNullOrEmpty(_tokenProvider.Token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenProvider.Token);
            }
            return client;
        }

        public async Task<List<InvoiceSummaryDto>> GetRecentCustomerInvoicesAsync(Guid customerId, int count)
        {
            try
            {
                var client = CreateAuthenticatedClient();

                // Passamos o count como query parameter para a API limitar os resultados
                var invoices = await client.GetFromJsonAsync<List<InvoiceSummaryDto>>($"api/customers/{customerId}/invoices?count={count}");
                if (invoices != null)
                {
                    return invoices;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter faturas: {ex.Message}. A carregar dados fictícios.");
            }

            // FAKE DATA de Fallback
            var fakeInvoices = new List<InvoiceSummaryDto>
        {
            new InvoiceSummaryDto { InvoiceNumber = "FT 2026/0045", IssueDate = DateTime.Now.AddDays(-5), DueDate = DateTime.Now.AddDays(25), TotalAmount = 750.00m, Status = "Unpaid" },
            new InvoiceSummaryDto { InvoiceNumber = "FT 2026/0012", IssueDate = DateTime.Now.AddDays(-40), DueDate = DateTime.Now.AddDays(-10), TotalAmount = 500.00m, Status = "Overdue" },
            new InvoiceSummaryDto { InvoiceNumber = "FT 2026/0003", IssueDate = DateTime.Now.AddDays(-60), DueDate = DateTime.Now.AddDays(-30), TotalAmount = 1250.00m, Status = "Paid" },
            new InvoiceSummaryDto { InvoiceNumber = "FT 2026/0002", IssueDate = DateTime.Now.AddDays(-90), DueDate = DateTime.Now.AddDays(-60), TotalAmount = 300.00m, Status = "Paid" },
            new InvoiceSummaryDto { InvoiceNumber = "FT 2026/0001", IssueDate = DateTime.Now.AddDays(-120), DueDate = DateTime.Now.AddDays(-90), TotalAmount = 1500.00m, Status = "Paid" }
        };

            // Usa o LINQ Take() para devolver apenas o número de faturas pedido (caso a API falhe)
            return fakeInvoices.Take(count).ToList();
        }

        public async Task<List<InvoiceProductDto>> GetActiveProducts()
        {
            try
            {
                var products = await _httpClientCatalog.GetFromJsonAsync<List<InvoiceProductDto>>("api/products/all/active");
                if (products != null)
                {
                    return products;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter produtos ativos: {ex.Message}. A carregar dados fictícios.");
            }
            return new List<InvoiceProductDto>();
        }


        public async Task<bool> CreateInvoiceAsync(InvoiceCreateDto invoice)
        {
            try
            {
                
                var response = await _httpClientBilling.PostAsJsonAsync("api/invoices", invoice);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                var erroApi = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Erro da API ({response.StatusCode}): {erroApi}");

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar fatura: {ex.Message}");
                return false;
            }
        }

    }
}