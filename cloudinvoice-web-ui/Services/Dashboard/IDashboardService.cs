using cloudinvoice_web_ui.DTOs.Dashboard;

namespace cloudinvoice_web_ui.Services.Dashboard
{
    public interface IDashboardService
    {
        Task<DashboardOverviewDto?> GetDashboardOverviewAsync();
    }
}
