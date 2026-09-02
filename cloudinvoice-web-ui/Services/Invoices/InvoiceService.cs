using System.Net.Http.Json;
using System.Net.Http.Headers;
using cloudinvoice_web_ui.Auth;
using cloudinvoice_web_ui.DTOs.Invoices;

namespace cloudinvoice_web_ui.Services.Invoices
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TokenProvider _tokenProvider;
        private readonly HttpClient _httpClientCatalog;

        public InvoiceService(IHttpClientFactory httpClientFactory, TokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
            _httpClientCatalog = _httpClientFactory.CreateClient("CatalogAPI");
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
            // FAKE DATA de Fallback
            return new List<InvoiceProductDto>
        {
            new InvoiceProductDto { Id = Guid.NewGuid(), Code = "SRV-001", Description = "Serviço de Consultoria", BasePrice = 100.00m, TaxRate = 23.00m },
            new InvoiceProductDto { Id = Guid.NewGuid(), Code = "DEV-001", Description = "Desenvolvimento de Software", BasePrice = 1500.00m, TaxRate = 23.00m },
            new InvoiceProductDto { Id = Guid.NewGuid(), Code = "MNT-001", Description = "Manutenção de Sistemas", BasePrice = 300.00m, TaxRate = 23.00m },
            new InvoiceProductDto { Id = Guid.NewGuid(), Code = "TRN-001", Description = "Treinamento Técnico", BasePrice = 200.00m, TaxRate = 23.00m }
        };
        }
    }
}