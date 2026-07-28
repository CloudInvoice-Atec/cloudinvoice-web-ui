using cloudinvoice_web_ui.Auth;
using cloudinvoice_web_ui.Models.Catalog;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace cloudinvoice_web_ui.Services.Catalog
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TokenProvider _tokenProvider;
        private readonly HttpClient _httpClientCatalog;

        public ProductService(IHttpClientFactory httpClientFactory, TokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;

            // Usa o Named Client exigido nas regras
            _httpClientCatalog = CreateAuthenticatedClient("CatalogAPI");
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

        public async Task<IEnumerable<ProductDto>> GetProductsAsync(ProductQueryParameters parameters)
        {
            try
            {
                // Construção da query string com base nos parâmetros
                var query = new List<string>
                {
                    $"page={parameters.Page}",
                    $"pageSize={parameters.PageSize}"
                };

                if (parameters.CategoryId.HasValue) query.Add($"categoryId={parameters.CategoryId.Value}");
                if (!string.IsNullOrEmpty(parameters.Search)) query.Add($"search={Uri.EscapeDataString(parameters.Search)}");
                if (parameters.IsActive.HasValue) query.Add($"isActive={parameters.IsActive.Value}");
                if (parameters.MinPrice.HasValue) query.Add($"minPrice={parameters.MinPrice.Value}");
                if (parameters.MaxPrice.HasValue) query.Add($"maxPrice={parameters.MaxPrice.Value}");

                var queryString = string.Join("&", query);

                return await _httpClientCatalog.GetFromJsonAsync<IEnumerable<ProductDto>>($"api/products?{queryString}")
                       ?? new List<ProductDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching products: {ex.Message}");
                return new List<ProductDto>(); // Propaga estado vazio para a UI tratar
            }
        }

        public async Task<ProductDto?> GetProductByIdAsync(Guid id)
        {
            try
            {
                return await _httpClientCatalog.GetFromJsonAsync<ProductDto>($"api/products/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching product {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateProductAsync(ProductCreateDto productDto)
        {
            try
            {
                var response = await _httpClientCatalog.PostAsJsonAsync("api/products", productDto);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating product: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateProductAsync(Guid id, ProductUpdateDto productDto)
        {
            try
            {
                var response = await _httpClientCatalog.PutAsJsonAsync($"api/products/{id}", productDto);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating product {id}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            try
            {
                var response = await _httpClientCatalog.DeleteAsync($"api/products/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting product {id}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeactivateProductAsync(Guid id)
        {
            try
            {
                var response = await _httpClientCatalog.PatchAsync($"api/products/{id}/deactivate", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deactivating product {id}: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
        {
            try
            {
                return await _httpClientCatalog.GetFromJsonAsync<IEnumerable<CategoryDto>>("api/categories")
                       ?? new List<CategoryDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching categories: {ex.Message}");
                return new List<CategoryDto>();
            }
        }
    }
}