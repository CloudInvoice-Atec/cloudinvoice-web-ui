using static cloudinvoice_web_ui.Components.Pages.Backoffice.Invoices.Invoice;

namespace cloudinvoice_web_ui.DTOs.Invoices
{
    public class InvoiceCreateDto
{
        public Guid? ClientId { get; set; }
        public string Reference { get; set; }
        public DateTime DateEmission { get; set; } = DateTime.Today;
        public DateTime DateDue { get; set; } = DateTime.Today.AddDays(30);
        public string Notes { get; set; }
        public List<InvoiceLineDto> Lines { get; set; } = new List<InvoiceLineDto>();
    }
}
