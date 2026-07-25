using System.ComponentModel.DataAnnotations;
using RestaurantManager.Models.Enums;

namespace RestaurantManager.Models;

public class DonGoiMon
{
    [Key]
    public int MaDon { get; set; }

    [Display(Name = "Bàn")]
    public int MaBan { get; set; }

    [Display(Name = "Khách hàng")]
    public int? MaKhachHang { get; set; }

    [Display(Name = "Nhân viên")]
    public int MaNhanVien { get; set; }

    [Display(Name = "Ngày tạo")]
    public DateTime NgayTao { get; set; } = DateTime.Now;

    [Display(Name = "Trạng thái")]
    public TrangThaiDon TrangThai { get; set; } = TrangThaiDon.DangPhucVu;

    public BanAn? BanAn { get; set; }
    public KhachHang? KhachHang { get; set; }
    public NguoiDung? NhanVien { get; set; }
    public ICollection<ChiTietDonGoiMon> ChiTietDonGoiMons { get; set; } = new List<ChiTietDonGoiMon>();
    public HoaDon? HoaDon { get; set; }
}
