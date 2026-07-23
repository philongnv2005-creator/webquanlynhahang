using Microsoft.EntityFrameworkCore;
using RestaurantManager.Data;
using RestaurantManager.Models.Enums;
using RestaurantManager.ViewModels;

namespace RestaurantManager.Services;

public class ReportService : IReportService
{
    private readonly AppDbContext _db;

    public ReportService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardViewModel> GetDashboardAsync()
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);
        var topDishes = await QueryTopDishes(today.AddDays(-30), tomorrow, 5);

        return new DashboardViewModel
        {
            TongBan = await _db.BanAns.CountAsync(x => x.TrangThai != TrangThaiBan.NgungSuDung),
            BanTrong = await _db.BanAns.CountAsync(x => x.TrangThai == TrangThaiBan.Trong),
            DatBanHomNay = await _db.DatBans.CountAsync(x => x.ThoiGianBatDau >= today && x.ThoiGianBatDau < tomorrow && x.TrangThai != TrangThaiDatBan.DaHuy),
            DonDangPhucVu = await _db.DonGoiMons.CountAsync(x => x.TrangThai == TrangThaiDon.DangPhucVu || x.TrangThai == TrangThaiDon.ChoThanhToan),
            DoanhThuHomNay = await _db.HoaDons.Where(x => x.TrangThai == TrangThaiHoaDon.DaThanhToan && x.NgayThanhToan >= today && x.NgayThanhToan < tomorrow).SumAsync(x => (decimal?)x.ThanhTien) ?? 0,
            MonBanChay = topDishes
        };
    }

    public async Task<ReportFilterViewModel> GetReportAsync(DateTime from, DateTime to)
    {
        var fromDate = from.Date;
        var toExclusive = to.Date.AddDays(1);
        var invoices = _db.HoaDons.Where(x =>
            x.TrangThai == TrangThaiHoaDon.DaThanhToan &&
            x.NgayThanhToan >= fromDate && x.NgayThanhToan < toExclusive);

        var daily = await invoices
            .GroupBy(x => x.NgayThanhToan.Date)
            .Select(g => new DailyRevenueViewModel
            {
                Ngay = g.Key,
                DoanhThu = g.Sum(x => x.ThanhTien),
                SoHoaDon = g.Count()
            })
            .OrderBy(x => x.Ngay)
            .ToListAsync();

        var customerCount = await invoices
            .Where(x => x.DonGoiMon!.MaKhachHang != null)
            .Select(x => x.DonGoiMon!.MaKhachHang)
            .Distinct()
            .CountAsync();

        return new ReportFilterViewModel
        {
            TuNgay = fromDate,
            DenNgay = to.Date,
            TongDoanhThu = await invoices.SumAsync(x => (decimal?)x.ThanhTien) ?? 0,
            SoHoaDon = await invoices.CountAsync(),
            SoKhach = customerCount,
            DoanhThuTheoNgay = daily,
            MonBanChay = await QueryTopDishes(fromDate, toExclusive, 10)
        };
    }

    private Task<List<TopDishViewModel>> QueryTopDishes(DateTime from, DateTime toExclusive, int take)
    {
        return _db.ChiTietDonGoiMons
            .Where(x => x.TrangThaiMon != TrangThaiMonTrongDon.DaHuy &&
                        x.DonGoiMon!.HoaDon != null &&
                        x.DonGoiMon.HoaDon.TrangThai == TrangThaiHoaDon.DaThanhToan &&
                        x.DonGoiMon.HoaDon.NgayThanhToan >= from &&
                        x.DonGoiMon.HoaDon.NgayThanhToan < toExclusive)
            .GroupBy(x => x.MonAn!.TenMon)
            .Select(g => new TopDishViewModel
            {
                TenMon = g.Key,
                SoLuong = g.Sum(x => x.SoLuong),
                DoanhThu = g.Sum(x => x.ThanhTien)
            })
            .OrderByDescending(x => x.SoLuong)
            .Take(take)
            .ToListAsync();
    }
}
