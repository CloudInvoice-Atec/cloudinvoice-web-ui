using cloudinvoice_web_ui.Enums;

namespace cloudinvoice_web_ui.DTOs.Invoices
{
    public class InvoiceResponseDto
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public InvoiceEnums.InvoiceStatus Status { get; set; }
        public InvoiceEnums.PaymentStatus PaymentStatus { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerTaxNumber { get; set; } = string.Empty;
        public decimal TotalBase { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
