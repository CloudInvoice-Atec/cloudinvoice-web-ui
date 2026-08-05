using System.ComponentModel.DataAnnotations;

namespace cloudinvoice_web_ui.Models.Catalog
{
    // DTO para a criação de um novo Produto
    public class ProductCreateDto
    {
        [Required(ErrorMessage = "The product code is required.")]
        [StringLength(20, ErrorMessage = "The code cannot exceed 20 characters.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "The description is required.")]
        [StringLength(200, ErrorMessage = "The description cannot exceed 200 characters.")]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, 1000000, ErrorMessage = "The base price must be greater than zero.")]
        public decimal BasePrice { get; set; }

        [Range(0, 100, ErrorMessage = "The tax rate must be between 0 and 100.")]
        public decimal TaxRate { get; set; }

        public UnitOfMeasure UnitOfMeasure { get; set; }

        public Guid CategoryId { get; set; }
    }
}
