using cloudinvoice_web_ui.Models.Catalog;

namespace cloudinvoice_web_ui.Services.Catalog
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync(ProductQueryParameters parameters);
        Task<ProductDto?> GetProductByIdAsync(Guid id);
        Task<bool> CreateProductAsync(ProductCreateDto productDto);
        Task<bool> UpdateProductAsync(Guid id, ProductUpdateDto productDto);
        Task<bool> DeleteProductAsync(Guid id);
        Task<bool> ToggleProductStatusAsync(Guid id);
        Task<IEnumerable<CategoryDto>> GetCategoriesAsync();
    }
}
