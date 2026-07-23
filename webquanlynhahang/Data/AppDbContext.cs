using Microsoft.EntityFrameworkCore;
using RestaurantManager.Models;

namespace RestaurantManager.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<VaiTro> VaiTros => Set<VaiTro>();
    public DbSet<NguoiDung> NguoiDungs => Set<NguoiDung>();
    public DbSet<KhachHang> KhachHangs => Set<KhachHang>();
    public DbSet<BanAn> BanAns => Set<BanAn>();
    public DbSet<DanhMucMonAn> DanhMucMonAns => Set<DanhMucMonAn>();
    public DbSet<MonAn> MonAns => Set<MonAn>();
    public DbSet<DatBan> DatBans => Set<DatBan>();
    public DbSet<DonGoiMon> DonGoiMons => Set<DonGoiMon>();
    public DbSet<ChiTietDonGoiMon> ChiTietDonGoiMons => Set<ChiTietDonGoiMon>();
    public DbSet<HoaDon> HoaDons => Set<HoaDon>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.ToTable("VaiTro");
            entity.HasIndex(x => x.TenVaiTro).IsUnique();
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.ToTable("NguoiDung");
            entity.HasIndex(x => x.TenDangNhap).IsUnique();
            entity.HasOne(x => x.VaiTro)
                .WithMany(x => x.NguoiDungs)
                .HasForeignKey(x => x.MaVaiTro)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.ToTable("KhachHang", table =>
                table.HasCheckConstraint("CK_KhachHang_SoDienThoai", "LEN([SoDienThoai]) BETWEEN 9 AND 15"));
            entity.HasIndex(x => x.SoDienThoai);
        });

        modelBuilder.Entity<BanAn>(entity =>
        {
            entity.ToTable("BanAn", table =>
                table.HasCheckConstraint("CK_BanAn_SoCho", "[SoCho] > 0"));
            entity.HasIndex(x => x.TenBan).IsUnique();
            entity.Property(x => x.TrangThai).HasConversion<string>().HasMaxLength(30);
        });

        modelBuilder.Entity<DanhMucMonAn>(entity =>
        {
            entity.ToTable("DanhMucMonAn");
            entity.HasIndex(x => x.TenDanhMuc).IsUnique();
        });

        modelBuilder.Entity<MonAn>(entity =>
        {
            entity.ToTable("MonAn", table =>
                table.HasCheckConstraint("CK_MonAn_DonGia", "[DonGia] > 0"));
            entity.Property(x => x.DonGia).HasPrecision(18, 2);
            entity.Property(x => x.TrangThai).HasConversion<string>().HasMaxLength(30);
            entity.HasIndex(x => new { x.MaDanhMuc, x.TenMon });
            entity.HasOne(x => x.DanhMucMonAn)
                .WithMany(x => x.MonAns)
                .HasForeignKey(x => x.MaDanhMuc)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DatBan>(entity =>
        {
            entity.ToTable("DatBan", table =>
            {
                table.HasCheckConstraint("CK_DatBan_ThoiGian", "[ThoiGianKetThuc] > [ThoiGianBatDau]");
                table.HasCheckConstraint("CK_DatBan_SoNguoi", "[SoNguoi] > 0");
            });
            entity.Property(x => x.TrangThai).HasConversion<string>().HasMaxLength(30);
            entity.HasIndex(x => new { x.MaBan, x.ThoiGianBatDau, x.ThoiGianKetThuc });
            entity.HasOne(x => x.KhachHang)
                .WithMany(x => x.DatBans)
                .HasForeignKey(x => x.MaKhachHang)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.BanAn)
                .WithMany(x => x.DatBans)
                .HasForeignKey(x => x.MaBan)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DonGoiMon>(entity =>
        {
            entity.ToTable("DonGoiMon");
            entity.Property(x => x.TrangThai).HasConversion<string>().HasMaxLength(30);
            entity.HasIndex(x => new { x.MaBan, x.TrangThai });
            entity.HasOne(x => x.BanAn)
                .WithMany(x => x.DonGoiMons)
                .HasForeignKey(x => x.MaBan)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.KhachHang)
                .WithMany(x => x.DonGoiMons)
                .HasForeignKey(x => x.MaKhachHang)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(x => x.NhanVien)
                .WithMany(x => x.DonGoiMons)
                .HasForeignKey(x => x.MaNhanVien)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ChiTietDonGoiMon>(entity =>
        {
            entity.ToTable("ChiTietDonGoiMon", table =>
            {
                table.HasCheckConstraint("CK_ChiTietDon_SoLuong", "[SoLuong] > 0");
                table.HasCheckConstraint("CK_ChiTietDon_DonGia", "[DonGia] > 0");
                table.HasCheckConstraint("CK_ChiTietDon_ThanhTien", "[ThanhTien] >= 0");
            });
            entity.Property(x => x.DonGia).HasPrecision(18, 2);
            entity.Property(x => x.ThanhTien).HasPrecision(18, 2);
            entity.Property(x => x.TrangThaiMon).HasConversion<string>().HasMaxLength(30);
            entity.HasIndex(x => new { x.MaDon, x.MaMon });
            entity.HasOne(x => x.DonGoiMon)
                .WithMany(x => x.ChiTietDonGoiMons)
                .HasForeignKey(x => x.MaDon)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.MonAn)
                .WithMany(x => x.ChiTietDonGoiMons)
                .HasForeignKey(x => x.MaMon)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.ToTable("HoaDon", table =>
            {
                table.HasCheckConstraint("CK_HoaDon_TongTien", "[TongTien] >= 0");
                table.HasCheckConstraint("CK_HoaDon_GiamGia", "[GiamGia] >= 0 AND [GiamGia] <= [TongTien]");
                table.HasCheckConstraint("CK_HoaDon_ThanhTien", "[ThanhTien] >= 0");
            });
            entity.Property(x => x.TongTien).HasPrecision(18, 2);
            entity.Property(x => x.GiamGia).HasPrecision(18, 2);
            entity.Property(x => x.ThanhTien).HasPrecision(18, 2);
            entity.Property(x => x.PhuongThuc).HasConversion<string>().HasMaxLength(50);
            entity.Property(x => x.TrangThai).HasConversion<string>().HasMaxLength(30);
            entity.HasIndex(x => x.MaDon).IsUnique();
            entity.HasIndex(x => x.NgayThanhToan);
            entity.HasOne(x => x.DonGoiMon)
                .WithOne(x => x.HoaDon)
                .HasForeignKey<HoaDon>(x => x.MaDon)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.NhanVien)
                .WithMany(x => x.HoaDons)
                .HasForeignKey(x => x.MaNhanVien)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
