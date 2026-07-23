using System.ComponentModel.DataAnnotations;

namespace RestaurantManager.ViewModels;

public class UserFormViewModel
{
    public int MaNguoiDung { get; set; }

    [Required, StringLength(50)]
    [Display(Name = "Tên đăng nhập")]
    public string TenDangNhap { get; set; } = string.Empty;

    [Required, StringLength(100)]
    [Display(Name = "Họ tên")]
    public string HoTen { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn vai trò.")]
    [Display(Name = "Vai trò")]
    public int MaVaiTro { get; set; }

    [MinLength(6)]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string? MatKhau { get; set; }

    [Display(Name = "Đang hoạt động")]
    public bool TrangThai { get; set; } = true;
}
