using cloudinvoice_web_ui.DTOs.Clientes;

namespace cloudinvoice_web_ui.Services.Customers
{
    public interface ICustomerService
{
        Task<CustomerProfileDto> GetCustomerProfileAsync(Guid id);
        Task<bool> UpdateCustomerAsync(Guid id, CustomerProfileDto customer);
    }
}
