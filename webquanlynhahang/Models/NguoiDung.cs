using System.ComponentModel.DataAnnotations;

namespace RestaurantManager.Models;

public class NguoiDung
{
    [Key]
    public int MaNguoiDung { get; set; }

    [Required, StringLength(50)]
    [Display(Name = "Tên đăng nhập")]
    public string TenDangNhap { get; set; } = string.Empty;

    [Required, StringLength(255)]
    public string MatKhauHash { get; set; } = string.Empty;

    [Required, StringLength(100)]
    [Display(Name = "Họ tên")]
    public string HoTen { get; set; } = string.Empty;

    [Display(Name = "Vai trò")]
    public int MaVaiTro { get; set; }

    [Display(Name = "Đang hoạt động")]
    public bool TrangThai { get; set; } = true;

    public VaiTro? VaiTro { get; set; }
    public ICollection<DonGoiMon> DonGoiMons { get; set; } = new List<DonGoiMon>();
    public ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
}
