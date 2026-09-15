using cloudinvoice_web_ui.Enums;
using static cloudinvoice_web_ui.Enums.InvoiceEnums;

namespace cloudinvoice_web_ui.DTOs.Invoices
{
    public class InvoiceResponseDto
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; }
        public string Reference { get; set; }

        
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }

        
        public InvoiceStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string Notes { get; set; }

        
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerTaxNumber { get; set; }
        public string CustomerAddress { get; set; }

        
        public string CompanyName { get; set; }
        public string CompanyTaxNumber { get; set; }
        public string CompanyAddress { get; set; }

        
        public decimal TotalBase { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalAmount { get; set; }

        
        public List<InvoiceLineResponseDto> Lines { get; set; } = new List<InvoiceLineResponseDto>();
    }

    public class InvoiceLineResponseDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }

        
        public string Description { get; set; }

        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal TaxRate { get; set; }

        
        public decimal LineTotal { get; set; }
    }
}