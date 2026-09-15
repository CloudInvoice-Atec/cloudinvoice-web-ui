namespace cloudinvoice_web_ui.DTOs.Invoices
{
    public class InvoiceQueryParametersDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
    }
}