using System.ComponentModel.DataAnnotations;
using RestaurantManager.Models.Enums;

namespace RestaurantManager.ViewModels;

public class CreateOrderViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn bàn.")]
    [Display(Name = "Bàn")]
    public int MaBan { get; set; }

    [Display(Name = "Khách hàng")]
    public int? MaKhachHang { get; set; }
}

public class AddOrderItemViewModel
{
    public int MaDon { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn món ăn.")]
    [Display(Name = "Món ăn")]
    public int MaMon { get; set; }

    [Range(1, 999)]
    [Display(Name = "Số lượng")]
    public int SoLuong { get; set; } = 1;
}

public class UpdateOrderItemViewModel
{
    public int MaChiTiet { get; set; }
    public int MaDon { get; set; }

    [Range(1, 999)]
    [Display(Name = "Số lượng")]
    public int SoLuong { get; set; }

    [Display(Name = "Trạng thái món")]
    public TrangThaiMonTrongDon TrangThaiMon { get; set; }
}
