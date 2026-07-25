using webquanlynhahang.ViewModels;

namespace webquanlynhahang.Services;

public interface IReportService
{
    Task<DashboardViewModel> GetDashboardAsync();
    Task<ReportFilterViewModel> GetReportAsync(DateTime from, DateTime to);
}
