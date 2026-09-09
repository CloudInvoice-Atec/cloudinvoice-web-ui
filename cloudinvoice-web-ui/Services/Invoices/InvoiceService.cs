using cloudinvoice_web_ui.Auth;
using cloudinvoice_web_ui.DTOs.Invoices;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace cloudinvoice_web_ui.Services.Invoices
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TokenProvider _tokenProvider;

        public InvoiceService(IHttpClientFactory httpClientFactory, TokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }

        private HttpClient GetBillingClient()
        {
            var client = _httpClientFactory.CreateClient("BillingAPI");
            var token = _tokenProvider.Token;

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        private HttpClient GetCatalogClient()
        {
            var client = _httpClientFactory.CreateClient("CatalogAPI");
            var token = _tokenProvider.Token;
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return client;
        }

        public async Task<bool> CreateInvoiceAsync(InvoiceCreateDto invoice)
        {
            try
            {
                var client = GetBillingClient();
                var response = await client.PostAsJsonAsync("api/invoices", invoice);

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<InvoiceSummaryDto>> GetRecentCustomerInvoicesAsync(Guid customerId, int count)
        {
            try
            {
                var client = GetBillingClient();
                var invoices = await client.GetFromJsonAsync<List<InvoiceSummaryDto>>($"api/customers/{customerId}/invoices?count={count}");
                return invoices ?? new List<InvoiceSummaryDto>();
            }
            catch (Exception)
            {
                return new List<InvoiceSummaryDto>();
            }
        }

        public async Task<InvoiceResponseDto?> GetInvoiceByIdAsync(Guid id)
        {
            try
            {
                var client = GetBillingClient();
                var response = await client.GetAsync($"api/Invoices/{id}");

                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadFromJsonAsync<InvoiceResponseDto>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<InvoicePagedResultDto<InvoiceResponseDto>?> GetInvoicesAsync(InvoiceQueryParametersDto parameters)
        {
            try
            {
                var client = GetBillingClient();
                var query = $"api/Invoices?pageNumber={parameters.PageNumber}&pageSize={parameters.PageSize}";
                var response = await client.GetAsync(query);

                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadFromJsonAsync<InvoicePagedResultDto<InvoiceResponseDto>>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<InvoiceProductDto>> GetActiveProducts()
        {
            try
            {
                var client = GetCatalogClient();
                var products = await client.GetFromJsonAsync<List<InvoiceProductDto>>("api/products/all/active");
                return products ?? new List<InvoiceProductDto>();
            }
            catch (Exception)
            {
                return new List<InvoiceProductDto>();
            }
        }

        public async Task<bool> UpdateInvoiceAsync(Guid id, InvoiceCreateDto invoiceUpdate)
        {
            try
            {
                var client = GetBillingClient();
                var response = await client.PutAsJsonAsync($"api/invoices/{id}", invoiceUpdate);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteInvoiceAsync(Guid id)
        {
            try
            {
                var client = GetBillingClient();
                var response = await client.DeleteAsync($"api/invoices/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> CancelInvoiceAsync(Guid id)
        {
            try
            {
                var client = GetBillingClient();

                var emptyContent = new StringContent("", System.Text.Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"api/invoices/{id}/cancel", emptyContent);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                var errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[CancelInvoice Falhou] Status: {response.StatusCode} | Erro: {errorMessage}");

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CancelInvoice Exceção]: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> MarkAsPaidAsync(Guid id)
        {
            try
            {
                var client = GetBillingClient();

                var emptyContent = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"api/invoices/{id}/pay", emptyContent);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                var errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[MarkAsPaid Falhou] Status: {response.StatusCode} | Erro: {errorMessage}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MarkAsPaid Exceção]: {ex.Message}");
                return false;
            }
        }
    }
}