using System.ComponentModel.DataAnnotations;
using RestaurantManager.Models.Enums;

namespace RestaurantManager.Models;

public class BanAn
{
    [Key]
    public int MaBan { get; set; }

    [Required, StringLength(50)]
    [Display(Name = "Tên bàn")]
    public string TenBan { get; set; } = string.Empty;

    [Required, StringLength(50)]
    [Display(Name = "Khu vực")]
    public string KhuVuc { get; set; } = "Khu chung";

    [Range(1, 100)]
    [Display(Name = "Số chỗ")]
    public int SoCho { get; set; }

    [Display(Name = "Trạng thái")]
    public TrangThaiBan TrangThai { get; set; } = TrangThaiBan.Trong;

    public ICollection<DatBan> DatBans { get; set; } = new List<DatBan>();
    public ICollection<DonGoiMon> DonGoiMons { get; set; } = new List<DonGoiMon>();
}
