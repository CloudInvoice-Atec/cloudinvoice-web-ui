using cloudinvoice_web_ui.DTOs.Clientes;
using cloudinvoice_web_ui.DTOs.Invoices;

namespace cloudinvoice_web_ui.Services.Customers
{
    public interface ICustomerService
{
        Task<CustomerProfileDto> GetCustomerProfileAsync(Guid id);
        Task<bool> UpdateCustomerAsync(Guid id, CustomerProfileDto customer);
        Task<Guid?> CreateCustomerAsync(InserirClienteDto customer);
        Task<PagedResultDto<CustomerProfileDto>?> GetCustomersAsync(CustomerQueryParameters parameters);
        Task<List<InvoiceSummaryDto>> GetCustomerInvoicesAsync(Guid id);
        Task<List<CustomerProfileDto>> GetCustomersActiveAsync();
    }
}
