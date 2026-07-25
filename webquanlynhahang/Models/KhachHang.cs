using System.ComponentModel.DataAnnotations;

namespace webquanlynhahang.Models;

public class KhachHang
{
    [Key]
    public int MaKhachHang { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "Họ tên")]
    public string HoTen { get; set; } = string.Empty;

    [Required, StringLength(15, MinimumLength = 9)]
    [RegularExpression(@"^[0-9]{9,15}$", ErrorMessage = "Số điện thoại phải gồm 9-15 chữ số.")]
    [Display(Name = "Số điện thoại")]
    public string SoDienThoai { get; set; } = string.Empty;

    [StringLength(255)]
    [Display(Name = "Địa chỉ")]
    public string? DiaChi { get; set; }

    [StringLength(255)]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    public ICollection<DatBan> DatBans { get; set; } = new List<DatBan>();
    public ICollection<DonGoiMon> DonGoiMons { get; set; } = new List<DonGoiMon>();
}
