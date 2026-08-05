using cloudinvoice_web_ui.Auth;
using cloudinvoice_web_ui.Models.Catalog;
using Models.Catalog_web_ui.Models.Catalog;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

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
                // Construção dinâmica da query string
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

                // 1. Criar as opções de configuração do JSON
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true, // Ignora diferenças de Maiúsculas/Minúsculas
                    Converters = { new JsonStringEnumConverter() } // Ensina o Blazor a converter Strings para Enums
                };

                // 2. Passar as opções diretamente no GetFromJsonAsync
                var response = await _httpClientCatalog.GetFromJsonAsync<PagedResultDto<ProductDto>>($"api/products?{queryString}", jsonOptions);

                // Devolve apenas a lista de items para a UI, ou uma lista vazia como fallback de segurança
                return response?.Items ?? new List<ProductDto>();
            }
            catch (Exception ex)
            {
                // Regra de Ouro: Propagar erro silenciosamente e não usar mocks
                Console.WriteLine($"Error fetching products: {ex.Message}");
                return new List<ProductDto>();
            }
        }

        public async Task<ProductDto?> GetProductByIdAsync(Guid id)
        {
            try
            {
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true, // Ignora diferenças de Maiúsculas/Minúsculas
                    Converters = { new JsonStringEnumConverter() } // Ensina o Blazor a converter Strings para Enums
                };
                // Certifica-te de que passas o _jsonOptions como segundo argumento!
                return await _httpClientCatalog.GetFromJsonAsync<ProductDto>($"api/products/{id}", jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching product {id}: {ex.Message}");
                return null; // Faz com que a UI apanhe o erro e mostre a caixa vermelha da imagem
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

        public async Task<bool> ToggleProductStatusAsync(Guid id)
        {
            try
            {
                var response = await _httpClientCatalog.PatchAsync($"api/products/{id}/toggle-status", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error toggling status for product {id}: {ex.Message}");
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