using cloudinvoice_web_ui.Models.Catalog;
using System.ComponentModel.DataAnnotations;

namespace cloudinvoice_web_ui.Models.Catalog
{
    // DTO principal para criação, edição e listagem de produtos
    public class ProductDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "The Code field is required.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Description field is required.")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Base Price must be greater than zero.")]
        public decimal BasePrice { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Tax Rate must be between 0 and 100.")]
        public decimal TaxRate { get; set; }

        [Required(ErrorMessage = "Unit of Measure is required.")]
        public UnitOfMeasure UnitOfMeasure { get; set; }

        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "Category is required.")]
        public Guid CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;
    }
}
