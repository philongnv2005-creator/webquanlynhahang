using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantManager.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BanAn",
                columns: table => new
                {
                    MaBan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenBan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KhuVuc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SoCho = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BanAn", x => x.MaBan);
                    table.CheckConstraint("CK_BanAn_SoCho", "[SoCho] > 0");
                });

            migrationBuilder.CreateTable(
                name: "DanhMucMonAn",
                columns: table => new
                {
                    MaDanhMuc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDanhMuc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucMonAn", x => x.MaDanhMuc);
                });

            migrationBuilder.CreateTable(
                name: "KhachHang",
                columns: table => new
                {
                    MaKhachHang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHang", x => x.MaKhachHang);
                    table.CheckConstraint("CK_KhachHang_SoDienThoai", "LEN([SoDienThoai]) BETWEEN 9 AND 15");
                });

            migrationBuilder.CreateTable(
                name: "VaiTro",
                columns: table => new
                {
                    MaVaiTro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenVaiTro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaiTro", x => x.MaVaiTro);
                });

            migrationBuilder.CreateTable(
                name: "MonAn",
                columns: table => new
                {
                    MaMon = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenMon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HinhAnh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MaDanhMuc = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonAn", x => x.MaMon);
                    table.CheckConstraint("CK_MonAn_DonGia", "[DonGia] > 0");
                    table.ForeignKey(
                        name: "FK_MonAn_DanhMucMonAn_MaDanhMuc",
                        column: x => x.MaDanhMuc,
                        principalTable: "DanhMucMonAn",
                        principalColumn: "MaDanhMuc",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DatBan",
                columns: table => new
                {
                    MaDatBan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhachHang = table.Column<int>(type: "int", nullable: false),
                    MaBan = table.Column<int>(type: "int", nullable: false),
                    ThoiGianBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoNguoi = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatBan", x => x.MaDatBan);
                    table.CheckConstraint("CK_DatBan_SoNguoi", "[SoNguoi] > 0");
                    table.CheckConstraint("CK_DatBan_ThoiGian", "[ThoiGianKetThuc] > [ThoiGianBatDau]");
                    table.ForeignKey(
                        name: "FK_DatBan_BanAn_MaBan",
                        column: x => x.MaBan,
                        principalTable: "BanAn",
                        principalColumn: "MaBan",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DatBan_KhachHang_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "KhachHang",
                        principalColumn: "MaKhachHang",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDung",
                columns: table => new
                {
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhauHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaVaiTro = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDung", x => x.MaNguoiDung);
                    table.ForeignKey(
                        name: "FK_NguoiDung_VaiTro_MaVaiTro",
                        column: x => x.MaVaiTro,
                        principalTable: "VaiTro",
                        principalColumn: "MaVaiTro",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DonGoiMon",
                columns: table => new
                {
                    MaDon = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaBan = table.Column<int>(type: "int", nullable: false),
                    MaKhachHang = table.Column<int>(type: "int", nullable: true),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonGoiMon", x => x.MaDon);
                    table.ForeignKey(
                        name: "FK_DonGoiMon_BanAn_MaBan",
                        column: x => x.MaBan,
                        principalTable: "BanAn",
                        principalColumn: "MaBan",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DonGoiMon_KhachHang_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "KhachHang",
                        principalColumn: "MaKhachHang",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DonGoiMon_NguoiDung_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDonGoiMon",
                columns: table => new
                {
                    MaChiTiet = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDon = table.Column<int>(type: "int", nullable: false),
                    MaMon = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TrangThaiMon = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietDonGoiMon", x => x.MaChiTiet);
                    table.CheckConstraint("CK_ChiTietDon_DonGia", "[DonGia] > 0");
                    table.CheckConstraint("CK_ChiTietDon_SoLuong", "[SoLuong] > 0");
                    table.CheckConstraint("CK_ChiTietDon_ThanhTien", "[ThanhTien] >= 0");
                    table.ForeignKey(
                        name: "FK_ChiTietDonGoiMon_DonGoiMon_MaDon",
                        column: x => x.MaDon,
                        principalTable: "DonGoiMon",
                        principalColumn: "MaDon",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietDonGoiMon_MonAn_MaMon",
                        column: x => x.MaMon,
                        principalTable: "MonAn",
                        principalColumn: "MaMon",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HoaDon",
                columns: table => new
                {
                    MaHoaDon = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDon = table.Column<int>(type: "int", nullable: false),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    NgayThanhToan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GiamGia = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PhuongThuc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDon", x => x.MaHoaDon);
                    table.CheckConstraint("CK_HoaDon_GiamGia", "[GiamGia] >= 0 AND [GiamGia] <= [TongTien]");
                    table.CheckConstraint("CK_HoaDon_ThanhTien", "[ThanhTien] >= 0");
                    table.CheckConstraint("CK_HoaDon_TongTien", "[TongTien] >= 0");
                    table.ForeignKey(
                        name: "FK_HoaDon_DonGoiMon_MaDon",
                        column: x => x.MaDon,
                        principalTable: "DonGoiMon",
                        principalColumn: "MaDon",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HoaDon_NguoiDung_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BanAn_TenBan",
                table: "BanAn",
                column: "TenBan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonGoiMon_MaDon_MaMon",
                table: "ChiTietDonGoiMon",
                columns: new[] { "MaDon", "MaMon" });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonGoiMon_MaMon",
                table: "ChiTietDonGoiMon",
                column: "MaMon");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucMonAn_TenDanhMuc",
                table: "DanhMucMonAn",
                column: "TenDanhMuc",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DatBan_MaBan_ThoiGianBatDau_ThoiGianKetThuc",
                table: "DatBan",
                columns: new[] { "MaBan", "ThoiGianBatDau", "ThoiGianKetThuc" });

            migrationBuilder.CreateIndex(
                name: "IX_DatBan_MaKhachHang",
                table: "DatBan",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_DonGoiMon_MaBan_TrangThai",
                table: "DonGoiMon",
                columns: new[] { "MaBan", "TrangThai" });

            migrationBuilder.CreateIndex(
                name: "IX_DonGoiMon_MaKhachHang",
                table: "DonGoiMon",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_DonGoiMon_MaNhanVien",
                table: "DonGoiMon",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_MaDon",
                table: "HoaDon",
                column: "MaDon",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_MaNhanVien",
                table: "HoaDon",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_NgayThanhToan",
                table: "HoaDon",
                column: "NgayThanhToan");

            migrationBuilder.CreateIndex(
                name: "IX_KhachHang_SoDienThoai",
                table: "KhachHang",
                column: "SoDienThoai");

            migrationBuilder.CreateIndex(
                name: "IX_MonAn_MaDanhMuc_TenMon",
                table: "MonAn",
                columns: new[] { "MaDanhMuc", "TenMon" });

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_MaVaiTro",
                table: "NguoiDung",
                column: "MaVaiTro");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_TenDangNhap",
                table: "NguoiDung",
                column: "TenDangNhap",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VaiTro_TenVaiTro",
                table: "VaiTro",
                column: "TenVaiTro",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietDonGoiMon");

            migrationBuilder.DropTable(
                name: "DatBan");

            migrationBuilder.DropTable(
                name: "HoaDon");

            migrationBuilder.DropTable(
                name: "MonAn");

            migrationBuilder.DropTable(
                name: "DonGoiMon");

            migrationBuilder.DropTable(
                name: "DanhMucMonAn");

            migrationBuilder.DropTable(
                name: "BanAn");

            migrationBuilder.DropTable(
                name: "KhachHang");

            migrationBuilder.DropTable(
                name: "NguoiDung");

            migrationBuilder.DropTable(
                name: "VaiTro");
        }
    }
}
