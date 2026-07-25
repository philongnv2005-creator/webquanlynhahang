using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webquanlynhahang.Data;
using webquanlynhahang.Helpers;
using webquanlynhahang.Models.Enums;
using webquanlynhahang.Services;
using webquanlynhahang.ViewModels;

namespace webquanlynhahang.Controllers;

[Authorize(Roles = RoleNames.ThanhToan)]
public class HoaDonController : Controller
{
    private readonly AppDbContext _db;
    private readonly IPaymentService _paymentService;

    public HoaDonController(AppDbContext db, IPaymentService paymentService)
    {
        _db = db;
        _paymentService = paymentService;
    }

    public async Task<IActionResult> Index(DateTime? from, DateTime? to)
    {
        var fromDate = from?.Date ?? DateTime.Today.AddDays(-30);
        var toExclusive = (to?.Date ?? DateTime.Today).AddDays(1);
        ViewBag.From = fromDate;
        ViewBag.To = toExclusive.AddDays(-1);
        var invoices = await _db.HoaDons
            .Include(x => x.DonGoiMon).ThenInclude(x => x!.BanAn)
            .Include(x => x.NhanVien)
            .Where(x => x.NgayThanhToan >= fromDate && x.NgayThanhToan < toExclusive)
            .OrderByDescending(x => x.NgayThanhToan)
            .ToListAsync();
        return View(invoices);
    }

    public async Task<IActionResult> Pending()
    {
        var orders = await _db.DonGoiMons
            .Include(x => x.BanAn)
            .Include(x => x.KhachHang)
            .Include(x => x.ChiTietDonGoiMons)
            .Where(x => x.TrangThai == TrangThaiDon.ChoThanhToan)
            .OrderBy(x => x.NgayTao)
            .ToListAsync();
        return View(orders);
    }

    [HttpGet]
    public async Task<IActionResult> Pay(int id)
    {
        var order = await _db.DonGoiMons
            .Include(x => x.BanAn)
            .Include(x => x.KhachHang)
            .Include(x => x.ChiTietDonGoiMons).ThenInclude(x => x.MonAn)
            .SingleOrDefaultAsync(x => x.MaDon == id && x.TrangThai == TrangThaiDon.ChoThanhToan);
        if (order is null) return NotFound();
        var total = order.ChiTietDonGoiMons.Where(x => x.TrangThaiMon != TrangThaiMonTrongDon.DaHuy).Sum(x => x.ThanhTien);
        ViewBag.Order = order;
        return View(new PaymentViewModel { MaDon = id, TongTien = total });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Pay(PaymentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await ReloadOrder(model.MaDon);
            return View(model);
        }

        try
        {
            var cashierId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var invoiceId = await _paymentService.PayAsync(model.MaDon, cashierId, model.GiamGia, model.PhuongThuc);
            TempData["Success"] = "Thanh toán thành công.";
            return RedirectToAction(nameof(Details), new { id = invoiceId });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await ReloadOrder(model.MaDon);
            return View(model);
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        var invoice = await _db.HoaDons
            .Include(x => x.NhanVien)
            .Include(x => x.DonGoiMon).ThenInclude(x => x!.BanAn)
            .Include(x => x.DonGoiMon).ThenInclude(x => x!.KhachHang)
            .Include(x => x.DonGoiMon).ThenInclude(x => x!.ChiTietDonGoiMons).ThenInclude(x => x.MonAn)
            .SingleOrDefaultAsync(x => x.MaHoaDon == id);
        return invoice is null ? NotFound() : View(invoice);
    }

    private async Task ReloadOrder(int id)
    {
        ViewBag.Order = await _db.DonGoiMons
            .Include(x => x.BanAn)
            .Include(x => x.KhachHang)
            .Include(x => x.ChiTietDonGoiMons).ThenInclude(x => x.MonAn)
            .SingleOrDefaultAsync(x => x.MaDon == id);
    }
}
