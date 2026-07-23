using System.ComponentModel.DataAnnotations;
using RestaurantManager.Models.Enums;

namespace RestaurantManager.ViewModels;

public class PaymentViewModel
{
    public int MaDon { get; set; }
    public decimal TongTien { get; set; }

    [Range(typeof(decimal), "0", "999999999", ErrorMessage = "Giảm giá không được âm.")]
    [Display(Name = "Giảm giá")]
    public decimal GiamGia { get; set; }

    [Display(Name = "Phương thức thanh toán")]
    public PhuongThucThanhToan PhuongThuc { get; set; } = PhuongThucThanhToan.TienMat;
}
