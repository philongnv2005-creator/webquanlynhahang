using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Data;
using RestaurantManager.Helpers;
using RestaurantManager.Models;
using RestaurantManager.ViewModels;

namespace RestaurantManager.Controllers;

[Authorize(Roles = RoleNames.QuanTriHoacQuanLy)]
public class MonAnController : Controller
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxFileSize = 5 * 1024 * 1024;
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _environment;

    public MonAnController(AppDbContext db, IWebHostEnvironment environment)
    {
        _db = db;
        _environment = environment;
    }

    public async Task<IActionResult> Index(string? q, int? categoryId)
    {
        var query = _db.MonAns.Include(x => x.DanhMucMonAn).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q)) query = query.Where(x => x.TenMon.Contains(q));
        if (categoryId.HasValue) query = query.Where(x => x.MaDanhMuc == categoryId.Value);
        ViewBag.Query = q;
        ViewBag.CategoryId = categoryId;
        ViewBag.Categories = new SelectList(await _db.DanhMucMonAns.OrderBy(x => x.TenDanhMuc).ToListAsync(), "MaDanhMuc", "TenDanhMuc", categoryId);
        return View(await query.OrderBy(x => x.DanhMucMonAn!.TenDanhMuc).ThenBy(x => x.TenMon).ToListAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadCategories();
        return View(new MonAnFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MonAnFormViewModel model)
    {
        await ValidateImage(model.HinhAnhFile);
        if (!ModelState.IsValid)
        {
            await LoadCategories(model.MaDanhMuc);
            return View(model);
        }

        var entity = new MonAn
        {
            TenMon = model.TenMon.Trim(),
            DonGia = model.DonGia,
            MoTa = model.MoTa,
            MaDanhMuc = model.MaDanhMuc,
            TrangThai = model.TrangThai,
            HinhAnh = await SaveImageAsync(model.HinhAnhFile)
        };
        _db.MonAns.Add(entity);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Thêm món ăn thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _db.MonAns.FindAsync(id);
        if (entity is null) return NotFound();
        await LoadCategories(entity.MaDanhMuc);
        return View(new MonAnFormViewModel
        {
            MaMon = entity.MaMon,
            TenMon = entity.TenMon,
            DonGia = entity.DonGia,
            MoTa = entity.MoTa,
            MaDanhMuc = entity.MaDanhMuc,
            TrangThai = entity.TrangThai,
            HinhAnhHienTai = entity.HinhAnh
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MonAnFormViewModel model)
    {
        if (id != model.MaMon) return BadRequest();
        await ValidateImage(model.HinhAnhFile);
        if (!ModelState.IsValid)
        {
            await LoadCategories(model.MaDanhMuc);
            return View(model);
        }

        var entity = await _db.MonAns.FindAsync(id);
        if (entity is null) return NotFound();
        entity.TenMon = model.TenMon.Trim();
        entity.DonGia = model.DonGia;
        entity.MoTa = model.MoTa;
        entity.MaDanhMuc = model.MaDanhMuc;
        entity.TrangThai = model.TrangThai;
        if (model.HinhAnhFile is not null)
        {
            DeleteImage(entity.HinhAnh);
            entity.HinhAnh = await SaveImageAsync(model.HinhAnhFile);
        }
        await _db.SaveChangesAsync();
        TempData["Success"] = "Cập nhật món ăn thành công.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadCategories(int? selected = null)
    {
        ViewBag.Categories = new SelectList(
            await _db.DanhMucMonAns.Where(x => x.TrangThai).OrderBy(x => x.TenDanhMuc).ToListAsync(),
            "MaDanhMuc", "TenDanhMuc", selected);
    }

    private Task ValidateImage(IFormFile? file)
    {
        if (file is null) return Task.CompletedTask;
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension)) ModelState.AddModelError("HinhAnhFile", "Chỉ chấp nhận JPG, PNG hoặc WEBP.");
        if (file.Length > MaxFileSize) ModelState.AddModelError("HinhAnhFile", "Ảnh không được vượt quá 5 MB.");
        if (!file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)) ModelState.AddModelError("HinhAnhFile", "Tệp tải lên không phải ảnh hợp lệ.");
        return Task.CompletedTask;
    }

    private async Task<string?> SaveImageAsync(IFormFile? file)
    {
        if (file is null || file.Length == 0) return null;
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var folder = Path.Combine(_environment.WebRootPath, "uploads", "monan");
        Directory.CreateDirectory(folder);
        await using var stream = System.IO.File.Create(Path.Combine(folder, fileName));
        await file.CopyToAsync(stream);
        return $"/uploads/monan/{fileName}";
    }

    private void DeleteImage(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return;
        var fullPath = Path.Combine(_environment.WebRootPath, relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
    }
}
