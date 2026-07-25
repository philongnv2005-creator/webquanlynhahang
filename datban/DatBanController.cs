using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Data;
using RestaurantManager.Helpers;
using RestaurantManager.Models;
using RestaurantManager.Models.Enums;
using RestaurantManager.Services;
using RestaurantManager.ViewModels;

namespace RestaurantManager.Controllers;

public class DatBanController : Controller
{
    private readonly AppDbContext _db;
    private readonly IDatBanService _service;

    public DatBanController(AppDbContext db, IDatBanService service)
    {
        _db = db;
        _service = service;
    }

    [Authorize(Roles = RoleNames.NoiBo)]
    public async Task<IActionResult> Index(DateTime? date, string? q)
    {
        var selectedDate = date?.Date ?? DateTime.Today;
        var nextDate = selectedDate.AddDays(1);
        var query = _db.DatBans.Include(x => x.BanAn).Include(x => x.KhachHang)
            .Where(x => x.ThoiGianBatDau >= selectedDate && x.ThoiGianBatDau < nextDate);
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(x => x.KhachHang!.HoTen.Contains(q) || x.KhachHang.SoDienThoai.Contains(q) || x.BanAn!.TenBan.Contains(q));
        ViewBag.Date = selectedDate;
        ViewBag.Query = q;
        return View(await query.OrderBy(x => x.ThoiGianBatDau).ToListAsync());
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> PublicCreate()
    {
        await LoadTables();
        return View(new DatBanFormViewModel
        {
            ThoiGianBatDau = DateTime.Now.AddHours(2),
            ThoiGianKetThuc = DateTime.Now.AddHours(4)
        });
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PublicCreate(DatBanFormViewModel model)
    {
        model.TrangThai = TrangThaiDatBan.ChoXacNhan;
        if (!ModelState.IsValid)
        {
            await LoadTables(model.MaBan);
            return View(model);
        }

        var result = await SaveReservation(model, null);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            await LoadTables(model.MaBan);
            return View(model);
        }
        TempData["Success"] = "Đã gửi yêu cầu đặt bàn. Nhà hàng sẽ xác nhận sớm.";
        return RedirectToAction("Index", "Home");
    }

    [Authorize(Roles = RoleNames.DatBan)]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadTables();
        await LoadCustomers();
        return View(new DatBanFormViewModel());
    }

    [Authorize(Roles = RoleNames.DatBan)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DatBanFormViewModel model)
    {
        await ApplySelectedCustomerAsync(model);
        if (!ModelState.IsValid)
        {
            await LoadTables(model.MaBan);
            await LoadCustomers(model.MaKhachHang);
            return View(model);
        }

        var result = await SaveReservation(model, null);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            await LoadTables(model.MaBan);
            await LoadCustomers(model.MaKhachHang);
            return View(model);
        }
        TempData["Success"] = "Tạo phiếu đặt bàn thành công.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.DatBan)]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _db.DatBans.Include(x => x.KhachHang).SingleOrDefaultAsync(x => x.MaDatBan == id);
        if (entity is null) return NotFound();
        await LoadTables(entity.MaBan);
        await LoadCustomers(entity.MaKhachHang);
        return View(new DatBanFormViewModel
        {
            MaDatBan = entity.MaDatBan,
            MaKhachHang = entity.MaKhachHang,
            HoTenKhach = entity.KhachHang!.HoTen,
            SoDienThoai = entity.KhachHang.SoDienThoai,
            MaBan = entity.MaBan,
            ThoiGianBatDau = entity.ThoiGianBatDau,
            ThoiGianKetThuc = entity.ThoiGianKetThuc,
            SoNguoi = entity.SoNguoi,
            TrangThai = entity.TrangThai,
            GhiChu = entity.GhiChu
        });
    }

    [Authorize(Roles = RoleNames.DatBan)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DatBanFormViewModel model)
    {
        if (id != model.MaDatBan) return BadRequest();
        await ApplySelectedCustomerAsync(model);
        if (!ModelState.IsValid)
        {
            await LoadTables(model.MaBan);
            await LoadCustomers(model.MaKhachHang);
            return View(model);
        }

        var result = await SaveReservation(model, id);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            await LoadTables(model.MaBan);
            await LoadCustomers(model.MaKhachHang);
            return View(model);
        }
        TempData["Success"] = "Cập nhật phiếu đặt bàn thành công.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.DatBan)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetStatus(int id, TrangThaiDatBan status)
    {
        var entity = await _db.DatBans.Include(x => x.BanAn).SingleOrDefaultAsync(x => x.MaDatBan == id);
        if (entity is null) return NotFound();
        entity.TrangThai = status;
        if (entity.BanAn is not null)
        {
            entity.BanAn.TrangThai = status switch
            {
                TrangThaiDatBan.DaXacNhan => TrangThaiBan.DaDat,
                TrangThaiDatBan.DaNhanBan => TrangThaiBan.DangPhucVu,
                _ => entity.BanAn.TrangThai
            };
        }
        await _db.SaveChangesAsync();
        if (status is TrangThaiDatBan.DaHuy or TrangThaiDatBan.HoanThanh)
            await _service.UpdateTableStatusAsync(entity.MaBan);
        return RedirectToAction(nameof(Index));
    }

    private async Task<(bool Success, string? Error)> SaveReservation(DatBanFormViewModel model, int? editId)
    {
        var customer = model.MaKhachHang.HasValue
            ? await _db.KhachHangs.FindAsync(model.MaKhachHang.Value)
            : await _db.KhachHangs.FirstOrDefaultAsync(x => x.SoDienThoai == model.SoDienThoai);

        if (customer is null)
        {
            customer = new KhachHang { HoTen = model.HoTenKhach.Trim(), SoDienThoai = model.SoDienThoai.Trim() };
            _db.KhachHangs.Add(customer);
            await _db.SaveChangesAsync();
        }
        else if (!string.IsNullOrWhiteSpace(model.HoTenKhach))
        {
            customer.HoTen = model.HoTenKhach.Trim();
        }

        var entity = editId.HasValue ? await _db.DatBans.FindAsync(editId.Value) : new DatBan();
        if (entity is null) return (false, "Phiếu đặt bàn không tồn tại.");
        var oldTableId = entity.MaBan;
        entity.MaKhachHang = customer.MaKhachHang;
        entity.MaBan = model.MaBan;
        entity.ThoiGianBatDau = model.ThoiGianBatDau;
        entity.ThoiGianKetThuc = model.ThoiGianKetThuc;
        entity.SoNguoi = model.SoNguoi;
        entity.TrangThai = model.TrangThai;
        entity.GhiChu = model.GhiChu;

        var validation = await _service.ValidateAsync(entity, editId);
        if (!validation.IsValid) return (false, validation.Error);

        if (!editId.HasValue) _db.DatBans.Add(entity);
        await _db.SaveChangesAsync();
        await _service.UpdateTableStatusAsync(entity.MaBan);
        if (editId.HasValue && oldTableId != entity.MaBan) await _service.UpdateTableStatusAsync(oldTableId);
        return (true, null);
    }

    private async Task ApplySelectedCustomerAsync(DatBanFormViewModel model)
    {
        if (!model.MaKhachHang.HasValue) return;

        var customer = await _db.KhachHangs.FindAsync(model.MaKhachHang.Value);
        if (customer is null)
        {
            ModelState.AddModelError(nameof(model.MaKhachHang), "Khách hàng đã chọn không tồn tại.");
            return;
        }

        model.HoTenKhach = customer.HoTen;
        model.SoDienThoai = customer.SoDienThoai;
        ModelState.Remove(nameof(model.HoTenKhach));
        ModelState.Remove(nameof(model.SoDienThoai));
    }

    private async Task LoadTables(int? selected = null)
    {
        ViewBag.Tables = new SelectList(
            await _db.BanAns.Where(x => x.TrangThai != TrangThaiBan.NgungSuDung).OrderBy(x => x.TenBan).ToListAsync(),
            "MaBan", "TenBan", selected);
    }

    private async Task LoadCustomers(int? selected = null)
    {
        ViewBag.Customers = new SelectList(
            await _db.KhachHangs.OrderBy(x => x.HoTen).ToListAsync(),
            "MaKhachHang", "HoTen", selected);
    }
}
