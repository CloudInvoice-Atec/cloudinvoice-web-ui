using cloudinvoice_web_ui.DTOs.Configuracoes;

namespace cloudinvoice_web_ui.Services.Settings
{
    public interface ICompanyService
{
        // Obtém os dados da empresa configurada
        Task<EmpresaDto> GetCompanySettingsAsync();

        // Guarda/Atualiza os dados da empresa
        Task<bool> SaveCompanySettingsAsync(EmpresaDto empresa);
    }
}
