namespace cloudinvoice_web_ui.DTOs.Invoices
{
    public class InvoiceSummaryDto
    {
        public string InvoiceNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } // Paid, Unpaid, Overdue
    }
}
