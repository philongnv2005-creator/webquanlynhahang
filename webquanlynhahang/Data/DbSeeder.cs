using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Helpers;
using RestaurantManager.Models;
using RestaurantManager.Models.Enums;

namespace RestaurantManager.Data;

public static class DbSeeder
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");

        try
        {
            var migrations = db.Database.GetMigrations();
            if (migrations.Any())
            {
                await db.Database.MigrateAsync();
            }
            else
            {
                await db.Database.EnsureCreatedAsync();
            }

            await SeedAsync(db);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Không thể khởi tạo cơ sở dữ liệu. Kiểm tra chuỗi kết nối SQL Server.");
        }
    }

    private static async Task SeedAsync(AppDbContext db)
    {
        if (!await db.VaiTros.AnyAsync())
        {
            db.VaiTros.AddRange(
                new VaiTro { TenVaiTro = RoleNames.QuanTriVien, MoTa = "Quản lý tài khoản và phân quyền" },
                new VaiTro { TenVaiTro = RoleNames.QuanLy, MoTa = "Quản lý dữ liệu nền và báo cáo" },
                new VaiTro { TenVaiTro = RoleNames.PhucVu, MoTa = "Đặt bàn và gọi món" },
                new VaiTro { TenVaiTro = RoleNames.ThuNgan, MoTa = "Thanh toán và hóa đơn" }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.NguoiDungs.AnyAsync())
        {
            var hasher = new PasswordHasher<NguoiDung>();
            var roleMap = await db.VaiTros.ToDictionaryAsync(x => x.TenVaiTro, x => x.MaVaiTro);
            var users = new[]
            {
                new NguoiDung { TenDangNhap = "admin", HoTen = "Quản trị hệ thống", MaVaiTro = roleMap[RoleNames.QuanTriVien], TrangThai = true },
                new NguoiDung { TenDangNhap = "quanly", HoTen = "Nguyễn Quản Lý", MaVaiTro = roleMap[RoleNames.QuanLy], TrangThai = true },
                new NguoiDung { TenDangNhap = "phucvu01", HoTen = "Trần Phục Vụ", MaVaiTro = roleMap[RoleNames.PhucVu], TrangThai = true },
                new NguoiDung { TenDangNhap = "thungan01", HoTen = "Lê Thu Ngân", MaVaiTro = roleMap[RoleNames.ThuNgan], TrangThai = true },
                new NguoiDung { TenDangNhap = "khoa01", HoTen = "Tài khoản bị khóa", MaVaiTro = roleMap[RoleNames.PhucVu], TrangThai = false }
            };

            foreach (var user in users)
            {
                user.MatKhauHash = hasher.HashPassword(user, "123456");
            }

            db.NguoiDungs.AddRange(users);
            await db.SaveChangesAsync();
        }

        if (!await db.DanhMucMonAns.AnyAsync())
        {
            db.DanhMucMonAns.AddRange(
                new DanhMucMonAn { TenDanhMuc = "Món khai vị", MoTa = "Các món dùng trước món chính" },
                new DanhMucMonAn { TenDanhMuc = "Món chính", MoTa = "Các món ăn chính" },
                new DanhMucMonAn { TenDanhMuc = "Đồ uống", MoTa = "Nước giải khát" },
                new DanhMucMonAn { TenDanhMuc = "Tráng miệng", MoTa = "Món ngọt sau bữa ăn" }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.MonAns.AnyAsync())
        {
            var cats = await db.DanhMucMonAns.ToDictionaryAsync(x => x.TenDanhMuc, x => x.MaDanhMuc);
            db.MonAns.AddRange(
                new MonAn { TenMon = "Gỏi cuốn", DonGia = 45000, MoTa = "Gỏi cuốn tôm thịt", MaDanhMuc = cats["Món khai vị"] },
                new MonAn { TenMon = "Cơm rang hải sản", DonGia = 85000, MoTa = "Cơm rang với tôm, mực và rau củ", MaDanhMuc = cats["Món chính"] },
                new MonAn { TenMon = "Lẩu Thái", DonGia = 350000, MoTa = "Lẩu chua cay dùng cho 4 người", MaDanhMuc = cats["Món chính"] },
                new MonAn { TenMon = "Gà nướng mật ong", DonGia = 220000, MoTa = "Gà nướng nguyên con", MaDanhMuc = cats["Món chính"] },
                new MonAn { TenMon = "Coca Cola", DonGia = 20000, MoTa = "Lon 330ml", MaDanhMuc = cats["Đồ uống"] },
                new MonAn { TenMon = "Trà đào", DonGia = 35000, MoTa = "Trà đào cam sả", MaDanhMuc = cats["Đồ uống"] },
                new MonAn { TenMon = "Kem flan", DonGia = 30000, MoTa = "Bánh flan caramel", MaDanhMuc = cats["Tráng miệng"] }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.BanAns.AnyAsync())
        {
            var tables = Enumerable.Range(1, 10).Select(i => new BanAn
            {
                TenBan = $"Bàn {i:00}",
                KhuVuc = i <= 5 ? "Tầng 1" : "Tầng 2",
                SoCho = i % 3 == 0 ? 8 : 4,
                TrangThai = TrangThaiBan.Trong
            });
            db.BanAns.AddRange(tables);
            await db.SaveChangesAsync();
        }

        if (!await db.KhachHangs.AnyAsync())
        {
            db.KhachHangs.AddRange(
                new KhachHang { HoTen = "Nguyễn Văn A", SoDienThoai = "0901000001", DiaChi = "Hà Nội" },
                new KhachHang { HoTen = "Trần Thị B", SoDienThoai = "0901000002", DiaChi = "Hà Nội" },
                new KhachHang { HoTen = "Lê Văn C", SoDienThoai = "0901000003", DiaChi = "Bắc Ninh" }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.DatBans.AnyAsync())
        {
            var customer = await db.KhachHangs.FirstAsync();
            var table = await db.BanAns.FirstAsync();
            var start = DateTime.Today.AddDays(1).AddHours(18);
            db.DatBans.Add(new DatBan
            {
                MaKhachHang = customer.MaKhachHang,
                MaBan = table.MaBan,
                ThoiGianBatDau = start,
                ThoiGianKetThuc = start.AddHours(2),
                SoNguoi = Math.Min(4, table.SoCho),
                TrangThai = TrangThaiDatBan.DaXacNhan,
                GhiChu = "Dữ liệu mẫu"
            });
            table.TrangThai = TrangThaiBan.DaDat;
            await db.SaveChangesAsync();
        }

        if (!await db.HoaDons.AnyAsync())
        {
            var table = await db.BanAns.OrderBy(x => x.MaBan).Skip(1).FirstAsync();
            var user = await db.NguoiDungs.FirstAsync(x => x.TenDangNhap == "phucvu01");
            var cashier = await db.NguoiDungs.FirstAsync(x => x.TenDangNhap == "thungan01");
            var dishes = await db.MonAns.Take(2).ToListAsync();

            var order = new DonGoiMon
            {
                MaBan = table.MaBan,
                MaNhanVien = user.MaNguoiDung,
                NgayTao = DateTime.Now.AddDays(-1),
                TrangThai = TrangThaiDon.DaThanhToan
            };
            foreach (var dish in dishes)
            {
                order.ChiTietDonGoiMons.Add(new ChiTietDonGoiMon
                {
                    MaMon = dish.MaMon,
                    SoLuong = 2,
                    DonGia = dish.DonGia,
                    ThanhTien = dish.DonGia * 2,
                    TrangThaiMon = TrangThaiMonTrongDon.DaPhucVu
                });
            }
            db.DonGoiMons.Add(order);
            await db.SaveChangesAsync();

            var total = order.ChiTietDonGoiMons.Sum(x => x.ThanhTien);
            db.HoaDons.Add(new HoaDon
            {
                MaDon = order.MaDon,
                MaNhanVien = cashier.MaNguoiDung,
                NgayThanhToan = DateTime.Now.AddDays(-1),
                TongTien = total,
                GiamGia = 0,
                ThanhTien = total,
                PhuongThuc = PhuongThucThanhToan.TienMat,
                TrangThai = TrangThaiHoaDon.DaThanhToan
            });
            await db.SaveChangesAsync();
        }
    }
}
