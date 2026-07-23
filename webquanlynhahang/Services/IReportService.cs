using RestaurantManager.ViewModels;

namespace RestaurantManager.Services;

public interface IReportService
{
    Task<DashboardViewModel> GetDashboardAsync();
    Task<ReportFilterViewModel> GetReportAsync(DateTime from, DateTime to);
}
