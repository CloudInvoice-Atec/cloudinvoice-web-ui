namespace cloudinvoice_web_ui.DTOs.Invoices
{
    public class InvoiceLineDto
{
        public Guid Id { get; set; } = Guid.NewGuid(); 
        public Guid? ProdutId { get; set; }
        public decimal Quantity { get; set; } = 1;
        public decimal BasePrice { get; set; } = 0;
        public decimal DiscountPercentage { get; set; } = 0;
        public decimal TaxRate { get; set; }
    }
}
