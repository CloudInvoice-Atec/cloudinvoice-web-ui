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

        // Método auxiliar seguro e confinado ao contexto deste utilizador
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
            // Se a Catalog API também precisar de token, descomenta as 3 linhas abaixo:
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
                // Agora o token vai garantidamente junto com o POST!
                var response = await client.PostAsJsonAsync("api/invoices", invoice);

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                // Regra 2: Devolve falso, a UI mostrará o erro ao utilizador
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
                return new List<InvoiceSummaryDto>(); // Sem fake data
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

        public async Task<IEnumerable<InvoiceResponseDto>?> GetInvoicesAsync(InvoiceQueryParametersDto parameters)
        {
            try
            {
                var client = GetBillingClient();
                var query = $"api/Invoices?pageNumber={parameters.PageNumber}&pageSize={parameters.PageSize}";
                var response = await client.GetAsync(query);

                if (!response.IsSuccessStatusCode)
                    return null;

                var pagedResult = await response.Content.ReadFromJsonAsync<InvoicePagedResultDto<InvoiceResponseDto>>();
                return pagedResult?.Items;
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
    }
}