using System.ComponentModel.DataAnnotations;

namespace RestaurantManager.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
    [Display(Name = "Tên đăng nhập")]
    public string TenDangNhap { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string MatKhau { get; set; } = string.Empty;

    [Display(Name = "Ghi nhớ đăng nhập")]
    public bool GhiNho { get; set; }

    public string? ReturnUrl { get; set; }
}

public class ChangePasswordViewModel
{
    [Required, DataType(DataType.Password)]
    [Display(Name = "Mật khẩu hiện tại")]
    public string MatKhauCu { get; set; } = string.Empty;

    [Required, MinLength(6), DataType(DataType.Password)]
    [Display(Name = "Mật khẩu mới")]
    public string MatKhauMoi { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    [Compare(nameof(MatKhauMoi), ErrorMessage = "Xác nhận mật khẩu không khớp.")]
    [Display(Name = "Xác nhận mật khẩu mới")]
    public string XacNhanMatKhau { get; set; } = string.Empty;
}
