using System.Data;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Data;
using RestaurantManager.Models;
using RestaurantManager.Models.Enums;

namespace RestaurantManager.Services;

public class PaymentService : IPaymentService
{
    private readonly AppDbContext _db;

    public PaymentService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<int> PayAsync(int orderId, int cashierId, decimal discount, PhuongThucThanhToan method)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        var order = await _db.DonGoiMons
            .Include(x => x.BanAn)
            .Include(x => x.HoaDon)
            .Include(x => x.ChiTietDonGoiMons)
            .SingleOrDefaultAsync(x => x.MaDon == orderId)
            ?? throw new InvalidOperationException("Đơn gọi món không tồn tại.");

        if (order.HoaDon is not null || order.TrangThai == TrangThaiDon.DaThanhToan)
        {
            throw new InvalidOperationException("Đơn đã được thanh toán.");
        }

        if (order.TrangThai != TrangThaiDon.ChoThanhToan)
        {
            throw new InvalidOperationException("Đơn cần chuyển sang trạng thái chờ thanh toán trước.");
        }

        var validItems = order.ChiTietDonGoiMons.Where(x => x.TrangThaiMon != TrangThaiMonTrongDon.DaHuy).ToList();
        if (validItems.Count == 0)
        {
            throw new InvalidOperationException("Đơn không có món hợp lệ để thanh toán.");
        }

        var total = validItems.Sum(x => x.ThanhTien);
        if (discount < 0 || discount > total)
        {
            throw new InvalidOperationException("Giảm giá không hợp lệ.");
        }

        var invoice = new HoaDon
        {
            MaDon = order.MaDon,
            MaNhanVien = cashierId,
            NgayThanhToan = DateTime.Now,
            TongTien = total,
            GiamGia = discount,
            ThanhTien = total - discount,
            PhuongThuc = method,
            TrangThai = TrangThaiHoaDon.DaThanhToan
        };

        _db.HoaDons.Add(invoice);
        order.TrangThai = TrangThaiDon.DaThanhToan;
        if (order.BanAn is not null)
        {
            order.BanAn.TrangThai = TrangThaiBan.Trong;
        }

        await _db.SaveChangesAsync();
        await transaction.CommitAsync();
        return invoice.MaHoaDon;
    }
}
