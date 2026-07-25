using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using webquanlynhahang.Models.Enums;

namespace webquanlynhahang.Models;

public class HoaDon
{
    [Key]
    public int MaHoaDon { get; set; }

    public int MaDon { get; set; }
    public int MaNhanVien { get; set; }

    [Display(Name = "Ngày thanh toán")]
    public DateTime NgayThanhToan { get; set; } = DateTime.Now;

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Tổng tiền")]
    public decimal TongTien { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Giảm giá")]
    public decimal GiamGia { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Thành tiền")]
    public decimal ThanhTien { get; set; }

    [Display(Name = "Phương thức")]
    public PhuongThucThanhToan PhuongThuc { get; set; } = PhuongThucThanhToan.TienMat;

    [Display(Name = "Trạng thái")]
    public TrangThaiHoaDon TrangThai { get; set; } = TrangThaiHoaDon.ChuaThanhToan;

    public DonGoiMon? DonGoiMon { get; set; }
    public NguoiDung? NhanVien { get; set; }
}
