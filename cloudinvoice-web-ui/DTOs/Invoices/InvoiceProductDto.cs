namespace cloudinvoice_web_ui.DTOs.Invoices
{
    public class InvoiceProductDto
{
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public decimal BasePrice { get; set; }
        public decimal TaxRate { get; set; }
        public string UnitOfMeasure { get; set; }
        public bool IsActive { get; set; }
    }
}
