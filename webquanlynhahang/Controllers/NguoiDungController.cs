using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Data;
using RestaurantManager.Helpers;
using RestaurantManager.Models;
using RestaurantManager.Services;
using RestaurantManager.ViewModels;

namespace RestaurantManager.Controllers;

[Authorize(Roles = RoleNames.QuanTriVien)]
public class NguoiDungController : Controller
{
    private readonly AppDbContext _db;
    private readonly IAuthService _authService;

    public NguoiDungController(AppDbContext db, IAuthService authService)
    {
        _db = db;
        _authService = authService;
    }

    public async Task<IActionResult> Index(string? q)
    {
        var query = _db.NguoiDungs.Include(x => x.VaiTro).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(x => x.TenDangNhap.Contains(q) || x.HoTen.Contains(q));
        ViewBag.Query = q;
        return View(await query.OrderBy(x => x.HoTen).ToListAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadRoles();
        return View(new UserFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserFormViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.MatKhau))
            ModelState.AddModelError(nameof(model.MatKhau), "Vui lòng nhập mật khẩu.");
        if (await _db.NguoiDungs.AnyAsync(x => x.TenDangNhap == model.TenDangNhap))
            ModelState.AddModelError(nameof(model.TenDangNhap), "Tên đăng nhập đã tồn tại.");
        if (!ModelState.IsValid)
        {
            await LoadRoles(model.MaVaiTro);
            return View(model);
        }

        var user = new NguoiDung
        {
            TenDangNhap = model.TenDangNhap.Trim(),
            HoTen = model.HoTen.Trim(),
            MaVaiTro = model.MaVaiTro,
            TrangThai = model.TrangThai
        };
        user.MatKhauHash = _authService.HashPassword(user, model.MatKhau!);
        _db.NguoiDungs.Add(user);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Tạo tài khoản thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _db.NguoiDungs.FindAsync(id);
        if (user is null) return NotFound();
        await LoadRoles(user.MaVaiTro);
        return View(new UserFormViewModel
        {
            MaNguoiDung = user.MaNguoiDung,
            TenDangNhap = user.TenDangNhap,
            HoTen = user.HoTen,
            MaVaiTro = user.MaVaiTro,
            TrangThai = user.TrangThai
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UserFormViewModel model)
    {
        if (id != model.MaNguoiDung) return BadRequest();
        var user = await _db.NguoiDungs.Include(x => x.VaiTro).SingleOrDefaultAsync(x => x.MaNguoiDung == id);
        if (user is null) return NotFound();
        if (await _db.NguoiDungs.AnyAsync(x => x.TenDangNhap == model.TenDangNhap && x.MaNguoiDung != id))
            ModelState.AddModelError(nameof(model.TenDangNhap), "Tên đăng nhập đã tồn tại.");
        if (!ModelState.IsValid)
        {
            await LoadRoles(model.MaVaiTro);
            return View(model);
        }

        var currentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (id == currentId && !model.TrangThai)
        {
            ModelState.AddModelError(nameof(model.TrangThai), "Bạn không thể tự khóa tài khoản đang đăng nhập.");
            await LoadRoles(model.MaVaiTro);
            return View(model);
        }

        var adminRole = await _db.VaiTros.SingleAsync(x => x.TenVaiTro == RoleNames.QuanTriVien);
        if (user.MaVaiTro == adminRole.MaVaiTro && (!model.TrangThai || model.MaVaiTro != adminRole.MaVaiTro))
        {
            var activeAdminCount = await _db.NguoiDungs.CountAsync(x => x.MaVaiTro == adminRole.MaVaiTro && x.TrangThai);
            if (activeAdminCount <= 1)
            {
                ModelState.AddModelError(string.Empty, "Không thể khóa hoặc hạ quyền quản trị viên đang hoạt động cuối cùng.");
                await LoadRoles(model.MaVaiTro);
                return View(model);
            }
        }

        user.TenDangNhap = model.TenDangNhap.Trim();
        user.HoTen = model.HoTen.Trim();
        user.MaVaiTro = model.MaVaiTro;
        user.TrangThai = model.TrangThai;
        if (!string.IsNullOrWhiteSpace(model.MatKhau))
            user.MatKhauHash = _authService.HashPassword(user, model.MatKhau);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Cập nhật tài khoản thành công.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadRoles(int? selected = null)
    {
        ViewBag.Roles = new SelectList(await _db.VaiTros.OrderBy(x => x.TenVaiTro).ToListAsync(), "MaVaiTro", "TenVaiTro", selected);
    }
}
