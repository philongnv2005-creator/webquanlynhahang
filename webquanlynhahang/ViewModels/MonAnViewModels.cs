using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using RestaurantManager.Models.Enums;

namespace RestaurantManager.ViewModels;

public class MonAnFormViewModel
{
    public int MaMon { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "Tên món")]
    public string TenMon { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "999999999", ErrorMessage = "Đơn giá phải lớn hơn 0.")]
    [Display(Name = "Đơn giá")]
    public decimal DonGia { get; set; }

    [StringLength(500)]
    [Display(Name = "Mô tả")]
    public string? MoTa { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn danh mục.")]
    [Display(Name = "Danh mục")]
    public int MaDanhMuc { get; set; }

    [Display(Name = "Trạng thái")]
    public TrangThaiMonAn TrangThai { get; set; } = TrangThaiMonAn.DangKinhDoanh;

    [Display(Name = "Ảnh món ăn")]
    public IFormFile? HinhAnhFile { get; set; }

    public string? HinhAnhHienTai { get; set; }
}
