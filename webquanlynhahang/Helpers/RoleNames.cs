namespace RestaurantManager.Helpers;

public static class RoleNames
{
    public const string QuanTriVien = "Quản trị viên";
    public const string QuanLy = "Quản lý";
    public const string PhucVu = "Nhân viên phục vụ";
    public const string ThuNgan = "Thu ngân";

    public const string QuanTriHoacQuanLy = QuanTriVien + "," + QuanLy;
    public const string NoiBo = QuanTriVien + "," + QuanLy + "," + PhucVu + "," + ThuNgan;
    public const string DatBan = QuanTriVien + "," + QuanLy + "," + PhucVu;
    public const string GoiMon = QuanLy + "," + PhucVu;
    public const string ThanhToan = QuanLy + "," + ThuNgan;
}
