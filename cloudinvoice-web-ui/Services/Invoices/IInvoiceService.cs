
using cloudinvoice_web_ui.DTOs.Invoices;
namespace cloudinvoice_web_ui.Services.Invoices
{
    public interface IInvoiceService
{
        Task<List<InvoiceSummaryDto>> GetRecentCustomerInvoicesAsync(Guid customerId, int count);
        Task<InvoiceResponseDto?> GetInvoiceByIdAsync(Guid id);
        Task<IEnumerable<InvoiceResponseDto>?> GetInvoicesAsync(InvoiceQueryParametersDto parameters);
    }
}
