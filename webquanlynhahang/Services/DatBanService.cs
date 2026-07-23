using Microsoft.EntityFrameworkCore;
using RestaurantManager.Data;
using RestaurantManager.Models;
using RestaurantManager.Models.Enums;

namespace RestaurantManager.Services;

public class DatBanService : IDatBanService
{
    private readonly AppDbContext _db;

    public DatBanService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(bool IsValid, string? Error)> ValidateAsync(DatBan datBan, int? ignoreId = null)
    {
        if (datBan.ThoiGianKetThuc <= datBan.ThoiGianBatDau)
        {
            return (false, "Thời gian kết thúc phải lớn hơn thời gian bắt đầu.");
        }

        var table = await _db.BanAns.FindAsync(datBan.MaBan);
        if (table is null || table.TrangThai == TrangThaiBan.NgungSuDung)
        {
            return (false, "Bàn không tồn tại hoặc đã ngừng sử dụng.");
        }

        if (datBan.SoNguoi > table.SoCho)
        {
            return (false, $"Số người vượt quá sức chứa của bàn ({table.SoCho} chỗ).");
        }

        var activeStates = new[]
        {
            TrangThaiDatBan.ChoXacNhan,
            TrangThaiDatBan.DaXacNhan,
            TrangThaiDatBan.DaNhanBan
        };

        var overlap = await _db.DatBans.AnyAsync(x =>
            x.MaBan == datBan.MaBan &&
            (!ignoreId.HasValue || x.MaDatBan != ignoreId.Value) &&
            activeStates.Contains(x.TrangThai) &&
            datBan.ThoiGianBatDau < x.ThoiGianKetThuc &&
            datBan.ThoiGianKetThuc > x.ThoiGianBatDau);

        return overlap
            ? (false, "Bàn đã có lịch đặt giao nhau trong khoảng thời gian này.")
            : (true, null);
    }

    public async Task UpdateTableStatusAsync(int tableId)
    {
        var table = await _db.BanAns.FindAsync(tableId);
        if (table is null || table.TrangThai is TrangThaiBan.DangPhucVu or TrangThaiBan.ChoThanhToan or TrangThaiBan.NgungSuDung)
        {
            return;
        }

        var now = DateTime.Now;
        var hasReservation = await _db.DatBans.AnyAsync(x =>
            x.MaBan == tableId &&
            (x.TrangThai == TrangThaiDatBan.ChoXacNhan || x.TrangThai == TrangThaiDatBan.DaXacNhan) &&
            x.ThoiGianKetThuc >= now);

        table.TrangThai = hasReservation ? TrangThaiBan.DaDat : TrangThaiBan.Trong;
        await _db.SaveChangesAsync();
    }
}
