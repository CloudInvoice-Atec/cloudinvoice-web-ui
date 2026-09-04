using cloudinvoice_web_ui.Enums;

using static cloudinvoice_web_ui.Components.Pages.Backoffice.Invoices.Invoice;
using static cloudinvoice_web_ui.Enums.InvoiceEnums;

namespace cloudinvoice_web_ui.DTOs.Invoices
{
    public class InvoiceCreateDto
{
        public Guid? CustomerId { get; set; } // Alterado para bater certo com a API
        public string Reference { get; set; }
        public DateTime DateEmission { get; set; } = DateTime.Today;
        public DateTime DateDue { get; set; } = DateTime.Today.AddDays(30);
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;
        public string Notes { get; set; }
        public List<InvoiceLineDto> Items { get; set; } = new List<InvoiceLineDto>();
    }
}
