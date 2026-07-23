using System.ComponentModel.DataAnnotations;
using RestaurantManager.Models.Enums;

namespace RestaurantManager.ViewModels;

public class DatBanFormViewModel
{
    public int MaDatBan { get; set; }

    [Display(Name = "Khách hàng có sẵn")]
    public int? MaKhachHang { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "Tên khách hàng")]
    public string HoTenKhach { get; set; } = string.Empty;

    [Required, RegularExpression(@"^[0-9]{9,15}$", ErrorMessage = "Số điện thoại phải gồm 9-15 chữ số.")]
    [Display(Name = "Số điện thoại")]
    public string SoDienThoai { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn bàn.")]
    [Display(Name = "Bàn")]
    public int MaBan { get; set; }

    [Required]
    [Display(Name = "Thời gian bắt đầu")]
    public DateTime ThoiGianBatDau { get; set; } = DateTime.Now.AddHours(1);

    [Required]
    [Display(Name = "Thời gian kết thúc")]
    public DateTime ThoiGianKetThuc { get; set; } = DateTime.Now.AddHours(3);

    [Range(1, 100)]
    [Display(Name = "Số người")]
    public int SoNguoi { get; set; } = 2;

    [Display(Name = "Trạng thái")]
    public TrangThaiDatBan TrangThai { get; set; } = TrangThaiDatBan.ChoXacNhan;

    [StringLength(255)]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }
}
