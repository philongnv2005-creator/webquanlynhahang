using Microsoft.EntityFrameworkCore;
using RestaurantManager.Data;
using RestaurantManager.Models;
using RestaurantManager.Models.Enums;

namespace RestaurantManager.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _db;

    public OrderService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<int> CreateOrderAsync(int tableId, int? customerId, int employeeId)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        var table = await _db.BanAns.SingleOrDefaultAsync(x => x.MaBan == tableId)
            ?? throw new InvalidOperationException("Bàn không tồn tại.");

        if (table.TrangThai == TrangThaiBan.NgungSuDung)
        {
            throw new InvalidOperationException("Bàn đã ngừng sử dụng.");
        }

        var hasActiveOrder = await _db.DonGoiMons.AnyAsync(x =>
            x.MaBan == tableId &&
            (x.TrangThai == TrangThaiDon.DangPhucVu || x.TrangThai == TrangThaiDon.ChoThanhToan));
        if (hasActiveOrder)
        {
            throw new InvalidOperationException("Bàn đang có một đơn gọi món chưa hoàn tất.");
        }

        var employeeActive = await _db.NguoiDungs.AnyAsync(x => x.MaNguoiDung == employeeId && x.TrangThai);
        if (!employeeActive)
        {
            throw new InvalidOperationException("Tài khoản nhân viên không hợp lệ.");
        }

        var order = new DonGoiMon
        {
            MaBan = tableId,
            MaKhachHang = customerId,
            MaNhanVien = employeeId,
            NgayTao = DateTime.Now,
            TrangThai = TrangThaiDon.DangPhucVu
        };

        _db.DonGoiMons.Add(order);
        table.TrangThai = TrangThaiBan.DangPhucVu;
        await _db.SaveChangesAsync();
        await transaction.CommitAsync();
        return order.MaDon;
    }

    public async Task AddItemAsync(int orderId, int dishId, int quantity)
    {
        if (quantity <= 0)
        {
            throw new InvalidOperationException("Số lượng phải lớn hơn 0.");
        }

        var order = await _db.DonGoiMons.FindAsync(orderId)
            ?? throw new InvalidOperationException("Đơn gọi món không tồn tại.");
        if (order.TrangThai != TrangThaiDon.DangPhucVu)
        {
            throw new InvalidOperationException("Chỉ có thể thêm món vào đơn đang phục vụ.");
        }

        var dish = await _db.MonAns.FindAsync(dishId)
            ?? throw new InvalidOperationException("Món ăn không tồn tại.");
        if (dish.TrangThai != TrangThaiMonAn.DangKinhDoanh)
        {
            throw new InvalidOperationException("Món ăn đã ngừng kinh doanh.");
        }

        var existing = await _db.ChiTietDonGoiMons.SingleOrDefaultAsync(x =>
            x.MaDon == orderId && x.MaMon == dishId && x.TrangThaiMon != TrangThaiMonTrongDon.DaHuy);

        if (existing is not null)
        {
            existing.SoLuong += quantity;
            existing.ThanhTien = existing.SoLuong * existing.DonGia;
        }
        else
        {
            _db.ChiTietDonGoiMons.Add(new ChiTietDonGoiMon
            {
                MaDon = orderId,
                MaMon = dishId,
                SoLuong = quantity,
                DonGia = dish.DonGia,
                ThanhTien = dish.DonGia * quantity,
                TrangThaiMon = TrangThaiMonTrongDon.ChoCheBien
            });
        }

        await _db.SaveChangesAsync();
    }

    public async Task UpdateItemAsync(int itemId, int quantity, TrangThaiMonTrongDon status)
    {
        var item = await _db.ChiTietDonGoiMons
            .Include(x => x.DonGoiMon)
            .SingleOrDefaultAsync(x => x.MaChiTiet == itemId)
            ?? throw new InvalidOperationException("Chi tiết món không tồn tại.");

        if (item.DonGoiMon?.TrangThai != TrangThaiDon.DangPhucVu)
        {
            throw new InvalidOperationException("Đơn không còn ở trạng thái có thể cập nhật.");
        }

        if (quantity <= 0)
        {
            throw new InvalidOperationException("Số lượng phải lớn hơn 0.");
        }

        item.SoLuong = quantity;
        item.ThanhTien = quantity * item.DonGia;
        item.TrangThaiMon = status;
        await _db.SaveChangesAsync();
    }

    public async Task CancelItemAsync(int itemId)
    {
        var item = await _db.ChiTietDonGoiMons
            .Include(x => x.DonGoiMon)
            .SingleOrDefaultAsync(x => x.MaChiTiet == itemId)
            ?? throw new InvalidOperationException("Chi tiết món không tồn tại.");

        if (item.DonGoiMon?.TrangThai != TrangThaiDon.DangPhucVu)
        {
            throw new InvalidOperationException("Không thể hủy món trong đơn đã chốt hoặc đã thanh toán.");
        }

        if (item.TrangThaiMon is TrangThaiMonTrongDon.DaPhucVu or TrangThaiMonTrongDon.DaHuy)
        {
            throw new InvalidOperationException("Món đã phục vụ hoặc đã hủy không thể hủy lại.");
        }

        item.TrangThaiMon = TrangThaiMonTrongDon.DaHuy;
        await _db.SaveChangesAsync();
    }

    public async Task MarkAwaitingPaymentAsync(int orderId)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        var order = await _db.DonGoiMons
            .Include(x => x.BanAn)
            .Include(x => x.ChiTietDonGoiMons)
            .SingleOrDefaultAsync(x => x.MaDon == orderId)
            ?? throw new InvalidOperationException("Đơn gọi món không tồn tại.");

        if (order.TrangThai != TrangThaiDon.DangPhucVu)
        {
            throw new InvalidOperationException("Đơn không ở trạng thái đang phục vụ.");
        }

        if (!order.ChiTietDonGoiMons.Any(x => x.TrangThaiMon != TrangThaiMonTrongDon.DaHuy))
        {
            throw new InvalidOperationException("Đơn phải có ít nhất một món hợp lệ trước khi thanh toán.");
        }

        order.TrangThai = TrangThaiDon.ChoThanhToan;
        if (order.BanAn is not null)
        {
            order.BanAn.TrangThai = TrangThaiBan.ChoThanhToan;
        }
        await _db.SaveChangesAsync();
        await transaction.CommitAsync();
    }
}
