
using cloudinvoice_web_ui.DTOs.Invoices;
namespace cloudinvoice_web_ui.Services.Invoices
{
    public interface IInvoiceService
{
        Task<List<InvoiceSummaryDto>> GetRecentCustomerInvoicesAsync(Guid customerId, int count);
        Task<InvoiceResponseDto?> GetInvoiceByIdAsync(Guid id);
        Task<IEnumerable<InvoiceResponseDto>?> GetInvoicesAsync(InvoiceQueryParametersDto parameters);
        Task<List<InvoiceProductDto>> GetActiveProducts();
        Task<bool> CreateInvoiceAsync(InvoiceCreateDto invoice);
        Task<bool> UpdateInvoiceAsync(Guid id, InvoiceCreateDto invoiceUpdate);

        Task<bool> DeleteInvoiceAsync(Guid id);
        Task<bool> CancelInvoiceAsync(Guid id);
        Task<bool> MarkAsPaidAsync(Guid id);

    }
}