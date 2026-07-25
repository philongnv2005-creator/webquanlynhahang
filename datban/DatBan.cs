using System.ComponentModel.DataAnnotations;
using RestaurantManager.Models.Enums;

namespace RestaurantManager.Models;

public class DatBan
{
    [Key]
    public int MaDatBan { get; set; }

    [Display(Name = "Khách hàng")]
    public int MaKhachHang { get; set; }

    [Display(Name = "Bàn")]
    public int MaBan { get; set; }

    [Required]
    [Display(Name = "Thời gian bắt đầu")]
    public DateTime ThoiGianBatDau { get; set; }

    [Required]
    [Display(Name = "Thời gian kết thúc")]
    public DateTime ThoiGianKetThuc { get; set; }

    [Range(1, 100)]
    [Display(Name = "Số người")]
    public int SoNguoi { get; set; }

    [Display(Name = "Trạng thái")]
    public TrangThaiDatBan TrangThai { get; set; } = TrangThaiDatBan.ChoXacNhan;

    [StringLength(255)]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    public KhachHang? KhachHang { get; set; }
    public BanAn? BanAn { get; set; }
}
