using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RestaurantManager.Models.Enums;

namespace RestaurantManager.Models;

public class MonAn
{
    [Key]
    public int MaMon { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "Tên món")]
    public string TenMon { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "999999999", ErrorMessage = "Đơn giá phải lớn hơn 0.")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Đơn giá")]
    public decimal DonGia { get; set; }

    [StringLength(500)]
    [Display(Name = "Mô tả")]
    public string? MoTa { get; set; }

    [StringLength(255)]
    [Display(Name = "Hình ảnh")]
    public string? HinhAnh { get; set; }

    [Display(Name = "Danh mục")]
    public int MaDanhMuc { get; set; }

    [Display(Name = "Trạng thái")]
    public TrangThaiMonAn TrangThai { get; set; } = TrangThaiMonAn.DangKinhDoanh;

    public DanhMucMonAn? DanhMucMonAn { get; set; }
    public ICollection<ChiTietDonGoiMon> ChiTietDonGoiMons { get; set; } = new List<ChiTietDonGoiMon>();
}
