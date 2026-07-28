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

        public CustomerService(IHttpClientFactory httpClientFactory, TokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
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

        public async Task<CustomerProfileDto> GetCustomerProfileAsync(Guid id)
        {
            try
            {
                var client = CreateAuthenticatedClient();

                var customer = await client.GetFromJsonAsync<CustomerProfileDto>($"api/customers/{id}");
                if (customer != null)
                {
                    return customer;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter cliente da API: {ex.Message}. A carregar dados fictícios.");
            }

            // FAKE DATA de Fallback
            return new CustomerProfileDto
            {
                Id = id,
                Name = "Tech Solutions, Lda (Mock)",
                TradeName = "TechSol",
                TaxId = "500123456",
                IsActive = true,
                CurrentDebt = 1250.00m,
                CreditLimit = 5000.00m,
                TotalInvoiced = 14500.00m,
                PaymentTermsDays = 30,
                Email = "geral@techsolutions.pt",
                Phone = "+351 253 111 222",
                Website = "https://www.techsolutions.pt",
                Address = "Rua da Inovação, Lote 45, Edifício A",
                City = "Guimarães",
                PostalCode = "4800-000",
                Country = "Portugal",
                DefaultDiscount = 5.0m,
                CreatedAt = new DateTime(2022, 5, 10),
                ContactPersonName = "João Silva",
                ContactPersonRole = "Diretor Financeiro",
                ContactPersonEmail = "joao.silva@techsolutions.pt",
                ContactPersonPhone = "+351 912 345 678"
            };
        }

        public async Task<List<InvoiceSummaryDto>> GetCustomerInvoicesAsync(Guid id)
        {
            try
            {
                var client = CreateAuthenticatedClient();                var invoices = await client.GetFromJsonAsync<List<InvoiceSummaryDto>>($"api/customers/{id}/invoices");
                if (invoices != null)
                {
                    return invoices;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter faturas da API: {ex.Message}. A carregar dados fictícios.");
            }

            // FAKE DATA de Fallback
            return new List<InvoiceSummaryDto>
        {
            new InvoiceSummaryDto { InvoiceNumber = "FT 2026/0045", IssueDate = DateTime.Now.AddDays(-5), DueDate = DateTime.Now.AddDays(25), TotalAmount = 750.00m, Status = "Unpaid" },
            new InvoiceSummaryDto { InvoiceNumber = "FT 2026/0012", IssueDate = DateTime.Now.AddDays(-40), DueDate = DateTime.Now.AddDays(-10), TotalAmount = 500.00m, Status = "Overdue" },
            new InvoiceSummaryDto { InvoiceNumber = "FT 2026/0003", IssueDate = DateTime.Now.AddDays(-60), DueDate = DateTime.Now.AddDays(-30), TotalAmount = 1250.00m, Status = "Paid" }
        };
        }

        public async Task<bool> UpdateCustomerAsync(Guid id, CustomerProfileDto customer)
        {
            try
            {
                var client = CreateAuthenticatedClient(); // ou "CustomersAPI"

                // Faz o PUT para a API
                var response = await client.PutAsJsonAsync($"api/customers/{id}", customer);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar cliente: {ex.Message}");
                return false; // Retorna falso para a UI saber que falhou (Fake Fallback)
            }
        }
    }
}
