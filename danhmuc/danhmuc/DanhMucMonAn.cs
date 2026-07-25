using System.ComponentModel.DataAnnotations;

namespace RestaurantManager.Models;

public class DanhMucMonAn
{
    [Key]
    public int MaDanhMuc { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "Tên danh mục")]
    public string TenDanhMuc { get; set; } = string.Empty;

    [StringLength(255)]
    [Display(Name = "Mô tả")]
    public string? MoTa { get; set; }

    [Display(Name = "Đang sử dụng")]
    public bool TrangThai { get; set; } = true;

    public ICollection<MonAn> MonAns { get; set; } = new List<MonAn>();
}
