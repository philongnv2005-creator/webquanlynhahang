using System.ComponentModel.DataAnnotations;

namespace webquanlynhahang.Models.Enums;

public enum TrangThaiBan
{
    [Display(Name = "Trống")]
    Trong,
    [Display(Name = "Đã đặt")]
    DaDat,
    [Display(Name = "Đang phục vụ")]
    DangPhucVu,
    [Display(Name = "Chờ thanh toán")]
    ChoThanhToan,
    [Display(Name = "Ngừng sử dụng")]
    NgungSuDung
}

public enum TrangThaiMonAn
{
    [Display(Name = "Đang kinh doanh")]
    DangKinhDoanh,
    [Display(Name = "Ngừng kinh doanh")]
    NgungKinhDoanh
}

public enum TrangThaiDatBan
{
    [Display(Name = "Chờ xác nhận")]
    ChoXacNhan,
    [Display(Name = "Đã xác nhận")]
    DaXacNhan,
    [Display(Name = "Đã nhận bàn")]
    DaNhanBan,
    [Display(Name = "Hoàn thành")]
    HoanThanh,
    [Display(Name = "Đã hủy")]
    DaHuy
}

public enum TrangThaiDon
{
    [Display(Name = "Đang phục vụ")]
    DangPhucVu,
    [Display(Name = "Chờ thanh toán")]
    ChoThanhToan,
    [Display(Name = "Đã thanh toán")]
    DaThanhToan,
    [Display(Name = "Đã hủy")]
    DaHuy
}

public enum TrangThaiMonTrongDon
{
    [Display(Name = "Chờ chế biến")]
    ChoCheBien,
    [Display(Name = "Đang chế biến")]
    DangCheBien,
    [Display(Name = "Đã phục vụ")]
    DaPhucVu,
    [Display(Name = "Đã hủy")]
    DaHuy
}

public enum TrangThaiHoaDon
{
    [Display(Name = "Chưa thanh toán")]
    ChuaThanhToan,
    [Display(Name = "Đã thanh toán")]
    DaThanhToan,
    [Display(Name = "Đã hủy")]
    DaHuy
}

public enum PhuongThucThanhToan
{
    [Display(Name = "Tiền mặt")]
    TienMat,
    [Display(Name = "Chuyển khoản")]
    ChuyenKhoan,
    [Display(Name = "Thẻ")]
    The
}
