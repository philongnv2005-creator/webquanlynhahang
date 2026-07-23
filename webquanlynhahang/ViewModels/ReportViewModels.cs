namespace RestaurantManager.ViewModels;

public class DashboardViewModel
{
    public int TongBan { get; set; }
    public int BanTrong { get; set; }
    public int DatBanHomNay { get; set; }
    public int DonDangPhucVu { get; set; }
    public decimal DoanhThuHomNay { get; set; }
    public IReadOnlyList<TopDishViewModel> MonBanChay { get; set; } = Array.Empty<TopDishViewModel>();
}

public class ReportFilterViewModel
{
    public DateTime TuNgay { get; set; } = DateTime.Today.AddDays(-30);
    public DateTime DenNgay { get; set; } = DateTime.Today;
    public decimal TongDoanhThu { get; set; }
    public int SoHoaDon { get; set; }
    public int SoKhach { get; set; }
    public IReadOnlyList<DailyRevenueViewModel> DoanhThuTheoNgay { get; set; } = Array.Empty<DailyRevenueViewModel>();
    public IReadOnlyList<TopDishViewModel> MonBanChay { get; set; } = Array.Empty<TopDishViewModel>();
}

public class DailyRevenueViewModel
{
    public DateTime Ngay { get; set; }
    public decimal DoanhThu { get; set; }
    public int SoHoaDon { get; set; }
}

public class TopDishViewModel
{
    public string TenMon { get; set; } = string.Empty;
    public int SoLuong { get; set; }
    public decimal DoanhThu { get; set; }
}
