using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Data;
using RestaurantManager.Helpers;
using RestaurantManager.Models.Enums;
using RestaurantManager.Services;
using RestaurantManager.ViewModels;

namespace RestaurantManager.Controllers;

[Authorize(Roles = RoleNames.GoiMon)]
public class DonGoiMonController : Controller
{
    private readonly AppDbContext _db;
    private readonly IOrderService _service;

    public DonGoiMonController(AppDbContext db, IOrderService service)
    {
        _db = db;
        _service = service;
    }

    public async Task<IActionResult> Index(TrangThaiDon? status)
    {
        var query = _db.DonGoiMons
            .Include(x => x.BanAn)
            .Include(x => x.KhachHang)
            .Include(x => x.NhanVien)
            .Include(x => x.ChiTietDonGoiMons)
            .AsQueryable();
        if (status.HasValue) query = query.Where(x => x.TrangThai == status.Value);
        ViewBag.Status = status;
        return View(await query.OrderByDescending(x => x.NgayTao).ToListAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadSelections();
        return View(new CreateOrderViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateOrderViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadSelections(model.MaBan, model.MaKhachHang);
            return View(model);
        }

        try
        {
            var employeeId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var orderId = await _service.CreateOrderAsync(model.MaBan, model.MaKhachHang, employeeId);
            TempData["Success"] = "Tạo đơn gọi món thành công.";
            return RedirectToAction(nameof(Details), new { id = orderId });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadSelections(model.MaBan, model.MaKhachHang);
            return View(model);
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _db.DonGoiMons
            .Include(x => x.BanAn)
            .Include(x => x.KhachHang)
            .Include(x => x.NhanVien)
            .Include(x => x.ChiTietDonGoiMons).ThenInclude(x => x.MonAn)
            .Include(x => x.HoaDon)
            .SingleOrDefaultAsync(x => x.MaDon == id);
        if (order is null) return NotFound();

        ViewBag.Dishes = new SelectList(
            await _db.MonAns.Where(x => x.TrangThai == TrangThaiMonAn.DangKinhDoanh)
                .OrderBy(x => x.TenMon).ToListAsync(),
            "MaMon", "TenMon");
        ViewBag.AddItem = new AddOrderItemViewModel { MaDon = id };
        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddItem(AddOrderItemViewModel model)
    {
        try
        {
            await _service.AddItemAsync(model.MaDon, model.MaMon, model.SoLuong);
            TempData["Success"] = "Đã thêm món vào đơn.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Details), new { id = model.MaDon });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateItem(UpdateOrderItemViewModel model)
    {
        try
        {
            await _service.UpdateItemAsync(model.MaChiTiet, model.SoLuong, model.TrangThaiMon);
            TempData["Success"] = "Đã cập nhật món.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Details), new { id = model.MaDon });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelItem(int id, int orderId)
    {
        try
        {
            await _service.CancelItemAsync(id);
            TempData["Success"] = "Đã hủy món.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Details), new { id = orderId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AwaitPayment(int id)
    {
        try
        {
            await _service.MarkAwaitingPaymentAsync(id);
            TempData["Success"] = "Đơn đã chuyển sang chờ thanh toán.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task LoadSelections(int? tableId = null, int? customerId = null)
    {
        ViewBag.Tables = new SelectList(
            await _db.BanAns.Where(x => x.TrangThai != TrangThaiBan.NgungSuDung).OrderBy(x => x.TenBan).ToListAsync(),
            "MaBan", "TenBan", tableId);
        ViewBag.Customers = new SelectList(
            await _db.KhachHangs.OrderBy(x => x.HoTen).ToListAsync(),
            "MaKhachHang", "HoTen", customerId);
    }
}
