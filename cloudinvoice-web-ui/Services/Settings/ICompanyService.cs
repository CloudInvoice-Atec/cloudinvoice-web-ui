using cloudinvoice_web_ui.DTOs.Configuracoes;

namespace cloudinvoice_web_ui.Services.Settings
{
    public interface ICompanyService
{
        
        Task<EmpresaDto> GetCompanySettingsAsync();

        
        Task<bool> SaveCompanySettingsAsync(EmpresaDto empresa);
    }
}
