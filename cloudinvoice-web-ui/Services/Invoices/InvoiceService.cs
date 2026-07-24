using System.Net.Http.Json;
using cloudinvoice_web_ui.DTOs.Invoices;

namespace cloudinvoice_web_ui.Services.Invoices
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public InvoiceService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<InvoiceSummaryDto>> GetRecentCustomerInvoicesAsync(Guid customerId, int count)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("BillingAPI");

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
    }
}